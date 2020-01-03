using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyHomeMeasure.Models;

namespace MyHomeMeasure.Services
{
    public interface ICosmosDbService
    {
        Task AddMeasureValue(MeasureValue value);
        Task<MeasureValue> GetMeasureValueAsync(string id);
        Task<IEnumerable<MeasureValue>> GetMeasureValuesAsync(DateTime? from = null, DateTime? to = null);
    }
}
