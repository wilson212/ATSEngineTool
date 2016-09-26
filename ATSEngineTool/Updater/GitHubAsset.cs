using Newtonsoft.Json;

namespace ATSEngineTool.Updater
{
    public class GitHubAsset
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public object Label { get; set; }

        [JsonProperty("uploader")]
        public GitHubUser Uploader { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("size")]
        public double Size { get; set; }

        [JsonProperty("download_count")]
        public int DownloadCount { get; set; }

        [JsonProperty("created_at")]
        public string Created { get; set; }

        [JsonProperty("updated_at")]
        public string Updated { get; set; }

        [JsonProperty("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
}
