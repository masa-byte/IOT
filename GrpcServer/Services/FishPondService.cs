using GrpcServer;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Services
{
    public class FishPondService : FishPond.FishPondBase
    {
        public Context Context { get; set; }

        public FishPondService(Context context)
        => Context = context;

        public override async Task<MultiPondData> GetAllData(Empty request, ServerCallContext context)
        {
            try
            {
                List<PondData> data = await Context.PondData.Take(5).ToListAsync();
                MultiPondData multiPondData = new MultiPondData();
                multiPondData.Data.AddRange(data);
                return await Task.FromResult(multiPondData);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }
        public override async Task<MultiPondData> GetDataByFishPondId(DataId request, ServerCallContext context)
        {
            try
            {
                List<PondData> data = await Context.PondData.Where(x => x.PondId == request.Id).Take(5).ToListAsync();
                MultiPondData multiPondData = new MultiPondData();
                multiPondData.Data.AddRange(data);
                return await Task.FromResult(multiPondData);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        public override async Task<PondData> GetDataByEntryId(DataId request, ServerCallContext context)
        {
            try
            {
                var pondData = await Context.PondData.Where(x => x.EntryId == request.Id).FirstOrDefaultAsync();
                if (pondData == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Data not found"));
                }
                return await Task.FromResult(pondData);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        public override async Task<PondData> CreateData(PondData request, ServerCallContext context)
        {
            try
            {
                PondData pondData = new PondData();
                pondData.PondId = request.PondId;
                pondData.TempC = request.TempC;
                pondData.Ph = request.Ph;
                pondData.FishWeightG = request.FishWeightG;
                pondData.FishLengthCm = request.FishLengthCm;
                pondData.CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow);
                pondData.AmmoniaGMl = request.AmmoniaGMl;
                pondData.NitriteGMl = request.NitriteGMl;
                pondData.DissolvedOxygenGMl = request.DissolvedOxygenGMl;
                pondData.EntryId = request.EntryId;
                pondData.TurbidityNtu = request.TurbidityNtu;
                pondData.Population = request.Population;

                await Context.PondData.AddAsync(pondData);
                await Context.SaveChangesAsync();
                return await Task.FromResult(pondData);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        public override async Task<PondData> UpdateData(PondData request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine(request);
                var pondData = await Context.PondData.FindAsync(request.EntryId);

                if (pondData == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Data not found"));
                }

                if (request.PondId != -1)
                    pondData.PondId = request.PondId;
                if (request.TempC != -1)
                    pondData.TempC = request.TempC;
                if (request.Ph != -1)
                    pondData.Ph = request.Ph;
                if (request.FishWeightG != -1)
                    pondData.FishWeightG = request.FishWeightG;
                if (request.FishLengthCm != -1)
                    pondData.FishLengthCm = request.FishLengthCm;
                if (request.AmmoniaGMl != -1)
                    pondData.AmmoniaGMl = request.AmmoniaGMl;
                if (request.NitriteGMl != -1)
                    pondData.NitriteGMl = request.NitriteGMl;
                if (request.DissolvedOxygenGMl != -1)
                    pondData.DissolvedOxygenGMl = request.DissolvedOxygenGMl;
                if (request.TurbidityNtu != -1)
                    pondData.TurbidityNtu = request.TurbidityNtu;
                if (request.Population != -1)
                    pondData.Population = request.Population;

                await Context.SaveChangesAsync();

                return await Task.FromResult(pondData);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        public override async Task<Empty> DeleteDataByFishPondId(DataId request, ServerCallContext context)
        {
            List<PondData> data = await Context.PondData.Where(x => x.PondId == request.Id).ToListAsync();
            if (data.Count == 0)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Data not found"));
            }

            Context.PondData.RemoveRange(data);
            await Context.SaveChangesAsync();
            return await Task.FromResult(new Empty());
        }

        public override async Task<Empty> DeleteDataByEntryId(DataId request, ServerCallContext context)
        {
            var pondData = await Context.PondData.Where(x => x.EntryId == request.Id).FirstOrDefaultAsync();
            if (pondData == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Data not found"));
            }

            Context.PondData.Remove(pondData);
            await Context.SaveChangesAsync();
            return await Task.FromResult(new Empty());
        }

        public override async Task<MultiPondData> MinDataValuesForDataDateRange(DataDateRange request, ServerCallContext context)
        {
            // Sorting all matched documents and then the first one has the smallest value
            MultiPondData multiPondData = new MultiPondData();

            var data = await Context.PondData
                    .Where(x => x.CreatedAt >= request.StartDate && x.CreatedAt <= request.EndDate)
                    .ToListAsync();

            if (data.Count == 0)
            {
                return await Task.FromResult(multiPondData);
            }

            // Mapping property name to the actual property name in the model
            request.PropertyName = MapPropertyName(request.PropertyName);

            // Grouping data based on the property value and then sorting them
            var orderedData = data
                .GroupBy(x => GetPropertyValue(x, request.PropertyName))
                .OrderBy(x => x.Key);
                
            var minData = orderedData.First().ToList();
       
            multiPondData.Data.AddRange(minData);
            return await Task.FromResult(multiPondData);
        }

        public override async Task<MultiPondData> MaxDataValuesForDataDateRange(DataDateRange request, ServerCallContext context)
        {
            // Sorting all matched documents and then the first one has the largest value
            MultiPondData multiPondData = new MultiPondData();

            var data = await Context.PondData
                    .Where(x => x.CreatedAt >= request.StartDate && x.CreatedAt <= request.EndDate)
                    .ToListAsync();

            if (data.Count == 0)
            {
                return await Task.FromResult(multiPondData);
            }

            // Mapping property name to the actual property name in the model
            request.PropertyName = MapPropertyName(request.PropertyName);

            // Grouping data based on the property value and then sorting them
            var orderedData = data
                .GroupBy(x => GetPropertyValue(x, request.PropertyName))
                .OrderByDescending(x => x.Key);

            var maxData = orderedData.First().ToList();

            multiPondData.Data.AddRange(maxData);
            return await Task.FromResult(multiPondData);
        }

        public override async Task<MultiAggPondData> AvgDataValuesForDataDateRange(DataDateRange request, ServerCallContext context)
        {
            MultiAggPondData multiAggPondData = new MultiAggPondData();

            var data = await Context.PondData
                    .Where(x => x.CreatedAt >= request.StartDate && x.CreatedAt <= request.EndDate)
                    .ToListAsync();

            if (data.Count == 0)
            {
                return await Task.FromResult(multiAggPondData);
            }

            // Mapping property name to the actual property name in the model
            request.PropertyName = MapPropertyName(request.PropertyName);

            // Dividing data into groups based on pond id
            var groupedData = data.GroupBy(x => x.PondId);

            // Finding the average value for each group
            foreach (var group in groupedData)
            {
                var avgValue = group.Average(x => Convert.ToDouble(GetPropertyValue(x, request.PropertyName)!));

                multiAggPondData.Data.Add(new AggPondData
                {
                    PondId = group.Key,
                    PropertyName = request.PropertyName,
                    AvgValue = (float)avgValue,
                    SumValue = 0
                });
            }

            return await Task.FromResult(multiAggPondData);
        }

        public override async Task<MultiAggPondData> SumDataValuesForDataDateRange(DataDateRange request, ServerCallContext context)
        {
            MultiAggPondData multiAggPondData = new MultiAggPondData();

            var data = await Context.PondData
                    .Where(x => x.CreatedAt >= request.StartDate && x.CreatedAt <= request.EndDate)
                    .ToListAsync();

            if (data.Count == 0)
            {
                return await Task.FromResult(multiAggPondData);
            }

            // Mapping property name to the actual property name in the model
            request.PropertyName = MapPropertyName(request.PropertyName);

            // Dividing data into groups based on pond id
            var groupedData = data.GroupBy(x => x.PondId);

            // Finding the sum value for each group
            foreach (var group in groupedData)
            {
                var sumValue = group.Sum(x => Convert.ToDouble(GetPropertyValue(x, request.PropertyName)!));

                multiAggPondData.Data.Add(new AggPondData
                {
                    PondId = group.Key,
                    PropertyName = request.PropertyName,
                    AvgValue = 0,
                    SumValue = (float)sumValue
                });
            }

            return await Task.FromResult(multiAggPondData);
        }


        private object? GetPropertyValue(PondData obj, string propertyName)
        {
            var property = typeof(PondData).GetProperty(propertyName);

            if (property != null)
            {
                return property.GetValue(obj)!;
            }

            return null;
        }

        private string MapPropertyName(string propertyName)
        {
            switch (propertyName)
            {
                case "temp_c":
                    return "TempC";
                case "ph":
                    return "Ph";
                case "fish_weight_g":
                    return "FishWeightG";
                case "fish_length_cm":
                    return "FishLengthCm";
                case "ammonia_g_ml":
                    return "AmmoniaGMl";
                case "nitrite_g_ml":
                    return "NitriteGMl";
                case "dissolved_oxygen_g_ml":
                    return "DissolvedOxygenGMl";
                case "turbidity_ntu":
                    return "TurbidityNtu";
                case "population":
                    return "Population";
                default:
                    return "TempC";
            }
        }
    }
}
