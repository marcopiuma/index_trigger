using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public class UVS_V2_Wrapper
{
    private HttpClient _httpClient;
    private readonly string cv_Endpoint;
    private readonly string cv_Key;
    private readonly string library_Name;

    private readonly string api_version = "2023-05-01-preview";

    internal static class FeatureModelName
    {
        static string VISION = "vision";
        static string SPEECH = "speech";
    }

    internal static class FeatuerModelDomain
    {
        static string GENERIC = "generic";
        static string MEDIAL = "medial";
        static string SURVEILLANCE = "surveillance";
    }


    internal class IndexRequestModel
    {
    }

    private IndexRequestModel createIngestionIndexRequestBody()
    {
        return null;
    }

    public UVS_V2_Wrapper(string cv_endpoint, string cv_key, string library)
    {
        _httpClient = new HttpClient();
        setDefaultHeaders();
        cv_Endpoint = cv_endpoint;
        cv_Key = cv_key;
        library_Name = library;
    }

    private readonly string retreival_path = "computervision/retrieval/indexes/";

    private void setDefaultHeaders()
    {
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cv_Key);
    }

    


    private async Task<bool> getIndex(string indexName)
    {
        var path = retreival_path + indexName;
        var url = string.Format("{0}{1}?{2}", cv_Endpoint, path, api_version);

        
        var response = await _httpClient.GetAsync(url);

        if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK) { return true; };
        return false;
    }

    private async Task<bool> createIndex(string indexName)
    {
        var path = retreival_path + indexName;
        var url = string.Format("{0}{1}?{2}", cv_Endpoint, path, api_version);

        var body = createIngestionIndexRequestBody();

        var response = await _httpClient.PutAsync(url,null);

        if (response != null && response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return true;    
        }
        return false;
    }

    private async Task<bool> generateIndexIfNotExists(string indexName)
    {
        if (await getIndex(indexName))
            return false;

        return await createIndex(indexName);
       
    }

    private async Task<string> generateIngestion(string indexName, string documentUrl)
    {
        return "";
    }


    public void addDocumentToIndex(string documentSaSUrl)
    {
        var indexExists = generateIndexIfNotExists(library_Name);
        var ingestion = generateIngestion(library_Name, documentSaSUrl);


    }
}



public class UVS_V1_Wrapper
{
    private HttpClient _httpClient;
    private readonly string cv_Endpoint;
    private readonly string cv_Key;
    private readonly string library_Name;
    private readonly string api_version = "2023-01-15-preview";

    public UVS_V1_Wrapper(string cv_endpoint, string cv_key, string library)
    {
        _httpClient = new HttpClient();
        cv_Endpoint = cv_endpoint;
        cv_Key = cv_key;
        library_Name = library;
    }

    private readonly string retreival_path = "vision-retrieval/retrieval/indexes/";



    public async Task<HttpResponseMessage> addDocumentToIndex(string SaSUrl, string documentId)
    {
        var path = retreival_path + library_Name;
        var url = string.Format("{0}{1}/documents/{2}?{3}", cv_Endpoint, path, documentId, api_version);

        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cv_Key);
        //_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

        JsonObject ingestionDocument = new JsonObject();
        ingestionDocument["documentUrl"] = SaSUrl;

        HttpContent content = new StringContent(ingestionDocument.ToJsonString());

        return await _httpClient.PutAsync(url, content);

    }
}