using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleNotificationService;
using PollLifx.Common.Model;

namespace PollLifx.NotifyChange
{
    public class ChangeProcessor : IChangeProcessor
    {
        public (bool success, string message) GetChanges(Lightbulb oldState, Lightbulb newState)
        {
            List<string> changes = new List<string>();

            if (oldState.power != newState.power)
                changes.Add($"turned {newState.power}");

            if (oldState.connected != newState.connected)
                changes.Add(newState.connected ? "connected" : "disconnected");

            var oldColour = CalculateColour(oldState.color);
            var newColour = CalculateColour(newState.color);

            if (oldColour != newColour)
                changes.Add($"changed to the colour {newColour}");

            if (!changes.Any())
                return (false, "");

            string changeString = ConcatChanges(changes);

            string message = $"{newState.label} light has been {changeString}";

            return (true, message);
        }

        private string ConcatChanges(List<string> changes)
        {
            if (changes.Count > 1)
                return string.Join(", ", changes.Take(changes.Count - 1)) + " and " + changes.Last();
            else
                return changes.SingleOrDefault();
        }

        private string CalculateColour(LightbulbColor lightbulbColor)
        {
            if (lightbulbColor.saturation < 0.2)
            {
                return "white";
            }

            var colours = new Dictionary<double, string>
            {
                { 0, "red" },
                { 36, "orange" },
                { 60, "yellow" },
                { 180, "cyan" },
                { 120, "green" },
                { 250, "blue" },
                { 280, "purple" },
                { 325, "pink" },
                { 360, "red" },
            };

            var closestColour = colours.OrderBy(colour => Math.Abs(lightbulbColor.hue - colour.Key)).First();

            return closestColour.Value;
        }
    }
}