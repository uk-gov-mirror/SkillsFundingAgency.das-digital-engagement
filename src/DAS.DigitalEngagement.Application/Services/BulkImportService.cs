﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAS.DigitalEngagement.Domain.DataCollection;
using DAS.DigitalEngagement.Domain.Mapping.BulkImport;
using DAS.DigitalEngagement.Domain.Services;
using DAS.DigitalEngagement.Models.BulkImport;
using Das.Marketo.RestApiClient.Interfaces;
using Das.Marketo.RestApiClient.Models;
using Microsoft.Extensions.Logging;
using Refit;

namespace DAS.DigitalEngagement.Application.Services
{
    public class BulkImportService : IBulkImportService
    {
        private readonly IChunkingService _chunkingService;
        private readonly IMarketoBulkImportClient _marketoBulkImportClient;
        private readonly ICsvService _csvService;
        private readonly ILogger<BulkImportService> _logger;
        private readonly IBulkImportStatusMapper _bulkImportStatusMapper;
        private readonly IBulkImportJobMapper _bulkImportJobMapper;

        public BulkImportService(IMarketoLeadClient marketoLeadClient,
            IMarketoBulkImportClient marketoBulkImportClient, ICsvService csvService, ILogger<BulkImportService> logger, IBulkImportStatusMapper bulkImportStatusMapper, IBulkImportJobMapper bulkImportJobMapper, IChunkingService chunkingService)
        {
            _marketoBulkImportClient = marketoBulkImportClient;
            _csvService = csvService;
            _logger = logger;
            _bulkImportStatusMapper = bulkImportStatusMapper;
            _bulkImportJobMapper = bulkImportJobMapper;
            _chunkingService = chunkingService;
        }


        public async Task<BulkImportStatus> ImportPeople<T>(IList<T> leads)
        {
            var fileStatus = new BulkImportStatus();

            var contactsChunks = _chunkingService.GetChunks(_csvService.GetByteCount(leads), leads).ToList();

            var index = 1;

            foreach (var contactsList in contactsChunks)
            {
                var importResult =
                    await ImportChunkedPeople(contactsList);
                fileStatus.BulkImportJobs.Add(importResult);

                _logger.LogInformation($"Bulk import chunk {index} of {contactsChunks.Count()} has been queued. \n Job details: {importResult} ");

                index++;
            }

            return fileStatus;
        }


        private async Task<BulkImportJob> ImportChunkedPeople<T>(IList<T> leads)
        {
            var csvStrings = _csvService.ToCsv(leads);    

            using (var stream = GenerateStreamFromString(csvStrings))
            {
                var streamPart = new StreamPart(stream, String.Empty, "text/csv");

                var bulkImportResponse = await _marketoBulkImportClient.PushLeads(streamPart);

                if (bulkImportResponse.Success == false)
                {
                    throw new Exception(
                        $"Unable to push person due to errors: {bulkImportResponse.ToString()}");
                }

                return bulkImportResponse.Result.Select(_bulkImportJobMapper.Map).FirstOrDefault();
            }
        }

        public async Task<BulkImportJob> ImportToCampaign<T>(IList<T> leads, string campaignId)
        {
            var csvStrings = _csvService.ToCsv(leads);

            using (var stream = GenerateStreamFromString(csvStrings))
            {

                var streamPart = new StreamPart(stream, String.Empty, "text/csv");

                var bulkImportResponse = await _marketoBulkImportClient.PushToProgram(streamPart,campaignId);

                if (bulkImportResponse.Success == false)
                {
                    throw new Exception(
                        $"Unable to push person to campaign {campaignId} due to errors: {bulkImportResponse.ToString()}");
                }
                
                return bulkImportResponse.Result.Select(_bulkImportJobMapper.Map).FirstOrDefault();
            }
        }

        public async Task<BulkImportJobStatus> GetJobStatus(int jobId)
        {
            var response = await _marketoBulkImportClient.GetStatus(jobId);

            if (response.Success == false)
            {
                _logger.LogError($"Error calling API to get bulk import job status. Details: {response.ToString()}");
            }

            var status = response.Result.Select(_bulkImportStatusMapper.Map).FirstOrDefault();

            if (status.NumOfRowsFailed > 0)
            {
                status.Failures = await GetFailures(status.Id);
            }

            if (status.NumOfRowsWithWarning > 0)
            {
                status.Warnings = await GetWarnings(status.Id);
            }

            return status;
        }

        public async Task<string> GetWarnings(int jobId)
        {
            var warningsResponse = await _marketoBulkImportClient.GetWarnings(jobId);

            return await warningsResponse.ReadAsStringAsync();
        }

        public async Task<string> GetFailures(int jobId)
        {
            var failureResponse = await _marketoBulkImportClient.GetFailures(jobId);

            return await failureResponse.ReadAsStringAsync();
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
