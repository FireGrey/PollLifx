using System;

namespace PollLifx.Model
{
    public class Lightbulb
    {
        public string id { get; set; }
        public Guid uuid { get; set; }
        public string label { get; set; }
        public bool connected  { get; set; }
        public string power { get; set; }
        public LightbulbColor color { get; set; }
        public LightbulbGroup group { get; set; }
        public LightbulbLocation location { get; set; }
        public DateTime last_seen { get; set; }
        public double seconds_since_seen { get; set; }
    }
}