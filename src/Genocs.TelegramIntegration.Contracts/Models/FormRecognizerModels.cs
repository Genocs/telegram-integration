using System.Collections.Generic;
using System;

namespace Genocs.TelegramIntegration.Contracts.Models
{
    public class FormRecognizerRequest
    {
        public string? Url { get; set; }
    }
    public class Classification
    {
        public string? Id { get; set; }
        public string? Project { get; set; }
        public string? Iteration { get; set; }
        public DateTime Created { get; set; }
        public List<Prediction>? Predictions { get; set; }
    }

    public class ContentDatum
    {
        public ValueConfidence? DocumentNumber { get; set; }
        public ValueConfidence? PartitaIVA { get; set; }
        public ValueConfidence? CRF0 { get; set; }
        public ValueConfidence? RefundAmount { get; set; }
        public ValueConfidence? InvoiceNumber { get; set; }
        public Goods Goods { get; set; }
    }

    public class ValueConfidence
    {
        public string? Value { get; set; }
        public double cConfidence { get; set; }
    }


    public class Goods
    {
        public object value { get; set; }
        public int confidence { get; set; }
    }


    public class Prediction
    {
        public double Probability { get; set; }
        public string TagId { get; set; }
        public string TagName { get; set; }
    }



    public class FormRecognizerResponse
    {
        public string ResourceUrl { get; set; }
        public Classification Classification { get; set; }
        public List<ContentDatum> ContentData { get; set; }
    }
}
