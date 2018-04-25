# PollLifx

PollLifx is a project which keeps an up to date record of the state of some lifx lightbulbs in dynamodb, and creates a notification on certain changes. This is just a small personal project that i'm using to experiment with different design patterns.

This project consists of:
* PollLifx.SyncState - a dotnet core lambda function project which retrieves all lights from the lifx api and stores their state in dynamodb
* cloudformation.yml - a cloudformation template for creating the required aws resources

The lambda function is designed to run as a cron and doesn't accept any input, nor does it return any output. Outside of the resources created by cloudformation, the function requires a valid lifx api key.