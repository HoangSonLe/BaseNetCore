using AutoMapper;
using Core.CommonModels.BaseModels;
using Elastic.Clients.Elasticsearch;
using Infrastructure.Elastic.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructure.Elastic
{
    public interface IBaseElasticSearchService
    {
        Task<Acknowledgement<List<FlightInfo>>> TestSimpleQuery();
        Task<Acknowledgement> PostSimpleData();
        Task<Acknowledgement<FlightInfo>> PostDataUsingClientElasticSearch();
        Task<Acknowledgement> UpdateDataUsingClientElasticSearch();
        Task<Acknowledgement> DeleteDataUsingClientElasticSearch();
        Task<Acknowledgement<List<FlightInfo>>> TestQueryUsingClientElasticSearch();
        Task<Acknowledgement<List<FlightInfo>>> TestQueryUsingNEST();
    }
    public class BaseElasticSearchService : IBaseElasticSearchService
    {
        private readonly ILogger<BaseElasticSearchService> _logger;
        private readonly IMapper _mapper;
        protected readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ElasticsearchClient _client;

        public BaseElasticSearchService(ILogger<BaseElasticSearchService> logger, IMapper mapper, IConfiguration configuration, IHttpClientFactory httpClientFactory, ElasticsearchClient client)
        {
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("ElasticsearchClient");
            _client = client;
        }

        private async Task<string> ExecuteElasticSearchQuery(string index, string query)
        {
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{index}/_search", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                var errorDetails = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Details: {errorDetails}");
            }

            // Throws exception if not a successful status code
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStringAsync().Result;

        }
        public async Task<HttpResponseMessage> PostToElasticsearchAsync<T>(string index, T data)
        {
            try
            {
                // Serialize the data to JSON
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                };
                var jsonData = JsonConvert.SerializeObject(data, settings);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // POST data to Elasticsearch
                var response = await _httpClient.PostAsync($"{index}/_doc", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Elasticsearch Error: {response.StatusCode} - {errorContent}");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting to Elasticsearch: {ex.Message}");
                throw;
            }
        }

        public async Task<Acknowledgement> PostSimpleData()
        {
            var indexName = _configuration.GetSection("ElasticSearch:IndexName").Value;
            var flight = new FlightInfo
            {
                FlightNum = "AA123",
                DestCountry = "USA",
                OriginWeather = "Sunny",
                OriginCityName = "New York",
                AvgTicketPrice = 500,
                DistanceMiles = 1000,
                FlightDelay = false,
                DestWeather = "Rainy",
                Dest = "Los Angeles",
                FlightDelayType = "No Delay",
                OriginCountry = "USA",
                DayOfWeek = 1,
                DistanceKilometers = 1609.34,
                Timestamp = DateTime.Now,
                DestLocation = new Location
                {
                    Lat = 34.0522,
                    Lon = -118.2437
                },
                DestAirportID = "LAX",
                Carrier = "American Airlines",
                Cancelled = false,
                FlightTimeMin = 120,
                Origin = "JFK",
                OriginLocation = new Location
                {
                    Lat = 40.7128,
                    Lon = -74.0060
                },
                DestRegion = "CA",
                OriginAirportID = "JFK",
                OriginRegion = "NY",
                DestCityName = "Los Angeles",
                FlightTimeHour = 2,
                FlightDelayMin = 0
            };
            var response = await PostToElasticsearchAsync<FlightInfo>(indexName, flight);
            if (response.IsSuccessStatusCode)
            {
                return new Acknowledgement()
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new Acknowledgement()
            {
                StatusCode = response.StatusCode
            };
        }

        public async Task<Acknowledgement<List<FlightInfo>>> TestQueryUsingClientElasticSearch()
        {
            var response = await _client.SearchAsync<FlightInfo>(s => s
                .Query(q => q
                    .Wildcard(m => m
                        .Field(f => f.FlightNum.Suffix("keyword"))
                        .Value("*AA123*")
                    )
                )
            );
            return new Acknowledgement<List<FlightInfo>>()
            {
                Data = response.Documents.ToList(),
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }

        public async Task<Acknowledgement<FlightInfo>> PostDataUsingClientElasticSearch()
        {
            var indexName = _configuration.GetSection("ElasticSearch:IndexName").Value;
            var flight = new FlightInfo
            {
                FlightNum = "AA123456",
                DestCountry = "USA",
                OriginWeather = "Sunny",
                OriginCityName = "New York",
                AvgTicketPrice = 500,
                DistanceMiles = 1000,
                FlightDelay = false,
                DestWeather = "Rainy",
                Dest = "Los Angeles",
                FlightDelayType = "No Delay",
                OriginCountry = "USA",
                DayOfWeek = 1,
                DistanceKilometers = 1609.34,
                Timestamp = DateTime.Now,
                DestLocation = new Location
                {
                    Lat = 34.0522,
                    Lon = -118.2437
                },
                DestAirportID = "LAX",
                Carrier = "American Airlines",
                Cancelled = false,
                FlightTimeMin = 120,
                Origin = "JFK",
                OriginLocation = new Location
                {
                    Lat = 40.7128,
                    Lon = -74.0060
                },
                DestRegion = "CA",
                OriginAirportID = "JFK",
                OriginRegion = "NY",
                DestCityName = "Los Angeles",
                FlightTimeHour = 2,
                FlightDelayMin = 0
            };

            var response = await _client.IndexAsync(flight, idx => idx.Index(indexName));
            if (response.IsValidResponse)
            {
                var getResponse = await _client.GetAsync<FlightInfo>(indexName, response.Id);
                return new Acknowledgement<FlightInfo>()
                {
                    Data = getResponse.Source,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new Acknowledgement<FlightInfo>()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ErrorMessageList = new List<string> { response.ElasticsearchServerError.Error.Reason.ToString() }
            };

        }
        public async Task<Acknowledgement> UpdateDataUsingClientElasticSearch()
        {
            var indexName = _configuration.GetSection("ElasticSearch:IndexName").Value;
            var flight = new FlightInfo
            {
                FlightNum = "AA123",
                OriginWeather = "Rainny",
            };
            var itemResponse = await _client.SearchAsync<FlightInfo>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.FlightNum)
                        .Query("AA123")
                    )
                )
            );
            if (itemResponse.Documents.Count == 0)
            {
                return new Acknowledgement()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    ErrorMessageList = new List<string> { "No data found" }
                };
            }
            var id = itemResponse.Hits.First().Id;
            var response = await _client.UpdateAsync<FlightInfo, FlightInfo>(indexName,id, u => u
                .Doc(flight)
            );
            if (response.IsValidResponse)
            {
                return new Acknowledgement()
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new Acknowledgement()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ErrorMessageList = new List<string> { response.ElasticsearchServerError.Error.Reason.ToString() }
            };
        }

        public async Task<Acknowledgement> DeleteDataUsingClientElasticSearch()
        {
            var indexName = _configuration.GetSection("ElasticSearch:IndexName").Value;
            var itemResponse = await _client.SearchAsync<FlightInfo>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.FlightNum)
                        .Query("AA123")
                    )
                )
            );
            if (itemResponse.Documents.Count == 0)
            {
                return new Acknowledgement()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    ErrorMessageList = new List<string> { "No data found" }
                };
            }
            var id = itemResponse.Hits.First().Id;
            var response = await _client.DeleteAsync<FlightInfo>(indexName,id);
            if (response.IsValidResponse)
            {
                return new Acknowledgement()
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new Acknowledgement()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ErrorMessageList = new List<string> { response.ElasticsearchServerError.Error.Reason.ToString() }
            };
        }
        public async Task<Acknowledgement<List<FlightInfo>>> TestQueryUsingNEST()
        {
            throw new NotImplementedException();
        }

        public async Task<Acknowledgement<List<FlightInfo>>> TestSimpleQuery()
        {
            var indexName = _configuration.GetSection("ElasticSearch:IndexName").Value;
            var query = @"{
                            ""query"": {
                                ""match"": {
                                       ""flightNum"":""AA123""
                                 }
                            }
                           }";
            var response = await ExecuteElasticSearchQuery(indexName, query);
            var result = JsonConvert.DeserializeObject<Root>(response);
            List<Hit> hits = result.hits.hits;
            List<FlightInfo> flightInfos = new List<FlightInfo>();
            foreach (var hit in hits)
            {
                flightInfos.Add(hit._source);
            }
            return new Acknowledgement<List<FlightInfo>>()
            {
                Data = flightInfos,
                StatusCode = System.Net.HttpStatusCode.OK
            };
        }

        public async Task<Acknowledgement<string>> TestAggregationQuery()
        {
            var indexName = _configuration.GetSection("ElasticSearch:IndexName").Value;
            var query = @"{
                            ""size"": 0,
                            ""aggs"": {
                                ""group_by_device"": {
                                    ""terms"": {
                                        ""field"": ""device.keyword""
                                    },
                                    ""aggs"": {
                                        ""group_by_vehicle"": {
                                            ""terms"": {
                                                ""field"": ""vehicle.keyword""
                                            },
                                            ""aggs"": {
                                                ""last_appearance"": {
                                                    ""max"": {
                                                        ""field"": ""timestamp""
                                                    }
                                                },
                                                ""last_document"": {
                                                    ""top_hits"": {
                                                        ""size"": 1,
                                                        ""sort"": [
                                                            {
                                                                ""timestamp"": {
                                                                    ""order"": ""desc""
                                                                }
                                                            }
                                                        ]
                                                    }
                                                },
                                                ""first_document"": {
                                                    ""top_hits"": {
                                                        ""size"": 1,
                                                        ""sort"": [
                                                            {
                                                                ""timestamp"": {
                                                                    ""order"": ""asc""
                                                                }
                                                            }
                                                        ]
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }";
            var response = await ExecuteElasticSearchQuery(indexName, query);
            return new Acknowledgement<string>()
            {
                Data = response,
                StatusCode = System.Net.HttpStatusCode.OK
            };

        }
}
