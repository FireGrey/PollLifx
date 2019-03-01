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

        // TODO
        private string CalculateColour(LightbulbColor lightbulbColor)
        {
            throw new NotImplementedException();
        }
    }
}