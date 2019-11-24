using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Types.Tenor {
    public class Nanomp4 {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("duration")]
        public double Duration { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Nanowebm {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Tinygif {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Tinymp4 {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("duration")]
        public double Duration { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Tinywebm {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Webm {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Gif {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Mp4 {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("duration")]
        public double Duration { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Loopedmp4 {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("duration")]
        public double Duration { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Mediumgif {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Nanogif {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("dims")]
        public List<int> Dims { get; set; }
        [JsonProperty("preview")]
        public string Preview { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }

    public class Medium {
        [JsonProperty("nanomp4")]
        public Nanomp4 Nanomp4 { get; set; }
        [JsonProperty("nanowebm")]
        public Nanowebm Nanowebm { get; set; }
        [JsonProperty("tinygif")]
        public Tinygif Tinygif { get; set; }
        [JsonProperty("tinymp4")]
        public Tinymp4 Tinymp4 { get; set; }
        [JsonProperty("tinywebm")]
        public Tinywebm Tinywebm { get; set; }
        [JsonProperty("webm")]
        public Webm Webm { get; set; }
        [JsonProperty("gif")]
        public Gif Gif { get; set; }
        [JsonProperty("mp4")]
        public Mp4 Mp4 { get; set; }
        [JsonProperty("loopedmp4")]
        public Loopedmp4 Loopedmp4 { get; set; }
        [JsonProperty("mediumgif")]
        public Mediumgif Mediumgif { get; set; }
        [JsonProperty("nanogif")]
        public Nanogif Nanogif { get; set; }
    }

    public class Result {
        [JsonProperty("tags")]
        public List<object> Tags { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("media")]
        public List<Medium> Media { get; set; }
        [JsonProperty("created")]
        public double Created { get; set; }
        [JsonProperty("shares")]
        public int Shares { get; set; }
        [JsonProperty("itemurl")]
        public string Itemurl { get; set; }
        [JsonProperty("composite")]
        public object Composite { get; set; }
        [JsonProperty("hasaudio")]
        public bool Hasaudio { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class TenorResult {
        [JsonProperty("weburl")]
        public string Weburl { get; set; }
        [JsonProperty("results")]
        public List<Result> Results { get; set; }
        [JsonProperty("next")]
        public string Next { get; set; }
    }

}
