using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleNotificationService;
using PollLifx.Common.Model;

namespace PollLifx.NotifyChange
{
    public interface IChangeProcessor
    {
        (bool success, string message) GetChanges(Lightbulb oldState, Lightbulb newState);
    }
}