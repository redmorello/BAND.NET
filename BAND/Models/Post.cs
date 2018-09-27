using System.Collections.Generic;
using Newtonsoft.Json;

namespace BAND.Models
{
    public class Post
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("author")]
        public Author Author { get; set; }
        [JsonProperty("post_key")]
        public string PostKey { get; set; }
        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }
        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }
        [JsonProperty("photos")]
        public List<Photo> Photos { get; set; }
        [JsonProperty("emotion_count")]
        public int EmotionCount { get; set; }
        [JsonProperty("latest_comments")]
        public List<Comment> LatestComments { get; set; }
        [JsonProperty("band_key")]
        public string BandKey { get; set; }
        [JsonProperty("post_read_count")]
        public int PostReadCount { get; set; }
        [JsonProperty("shared_count")]
        public int SharedCount { get; set; }
        [JsonProperty("is_multilingual")]
        public bool IsMultilingual { get; set; }
    }
}