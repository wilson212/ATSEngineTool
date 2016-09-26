using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ATSEngineTool.Updater
{
    public class GitHubRelease
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("assets_url")]
        public string AssetsUrl { get; set; }

        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("target_commitish")]
        public string TargetCommitish { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("draft")]
        public bool Draft { get; set; }

        [JsonProperty("author")]
        public GitHubUser Author { get; set; }

        [JsonProperty("prerelease")]
        public bool PreRelease { get; set; }

        [JsonProperty("created_at")]
        public DateTime Created { get; set; }

        [JsonProperty("published_at")]
        public DateTime Published { get; set; }

        [JsonProperty("assets")]
        public IList<GitHubAsset> Assets { get; set; }

        [JsonProperty("tarball_url")]
        public string TarballUrl { get; set; }

        [JsonProperty("zipball_url")]
        public string ZipballUrl { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
