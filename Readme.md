# PollLifx

PollLifx is a project which keeps an up to date record of the state of some lifx lightbulbs in dynamodb. This was created so that I can handle changes to the state of my lifx lightbulbs.

This project consists of:
* PollLifx - a dotnet core lambda function project which retrieves all lights from the lifx api and stores their state in dynamodb
* cloudformation.yml - a cloudformation template for creating the required aws resources

The lambda function is designed to run as a cron and doesn't accept any input, nor does it return any output. Outside of the resources created by cloudformation, the function requires a valid lifx api key.