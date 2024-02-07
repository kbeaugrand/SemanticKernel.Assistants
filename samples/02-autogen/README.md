## Auto-Gen assistant

This sample is based on the approach proposed by Auto-Gen.
It is realized through 2 assistants working together to code and execute the code needed to respond to user requests.

- __AssistantAgent (NL 2 Code)__: this agent takes charge of the user's request and produces Python code to respond to the user's request.
- __CodeInterpreter__: This agent takes as input the various parameters required to execute the Python code supplied by the AssistantAgent. Through its native plugin, it interacts with Docker to start a container, install the necessary dependencies and execute the Python code in this container, then returns the result.

## Samples

With this code, you can start by requesting some actions.

### Settings

To run this sample, you will need to set your settings: 

**appsettings.development.json**: 
```json
{
  "AzureOpenAIEndpoint": "<Your Azure OpenAI endpoint>",
  "AzureOpenAIAPIKey": "<Your Azure OpenAI Key>",
  "OllamaEndpoint": "http://localhost:11434",
  "AzureOpenAIGPT4DeploymentName": "gpt-4-1106-preview",
  "AzureOpenAIGPT35DeploymentName": "gpt-35-turbo-16k",
  "CodeInterpreter": {
    "DockerEndpoint": "npipe://./pipe/docker_engine",
    "DockerImage": "python:3-alpine",
    "OutputFilePath": "C:\\auto-gen\\outputs"
  }
}
```

### Video games sales

#### Read an provide calculations about the file

```
**User** > Given file vgsales.csv, which region has performed the best in terms of sales? What are results of each regions?
**AutoGen** > The sales results for each region are as follows:
- North America: 4392.95 million
- Europe: 2434.13 million
- Japan: 1291.02 million
- Other: 797.75 million
- Global: 8920.44 million

The region with the highest sales is Global with 8920.44 million.
```

To give this results, the autogen realized those actions: 

```
10:03:54:579	_02_autogen.Plugins.CodeInterpretionPlugin: Debug: Creating container.
10:03:54:826	_02_autogen.Plugins.CodeInterpretionPlugin: Debug: Starting the container (id: 425900d75443a2f7f2f58f05f18d48d31a06372f8bf839fafa1fd29ab9c5632b).
10:03:55:455	requirements.txt:
10:03:55:455	pandas
10:03:55:455	code.py:
10:03:55:455	import pandas as pd
10:03:55:455	
10:03:55:455	# Load the dataset
10:03:55:455	file_path = '/var/app/inputs/vgsales.csv'
10:03:55:455	data = pd.read_csv(file_path)
10:03:55:455	
10:03:55:455	# Sum sales by region
10:03:55:455	na_sales = data['NA_Sales'].sum()
10:03:55:455	eu_sales = data['EU_Sales'].sum()
10:03:55:455	jp_sales = data['JP_Sales'].sum()
10:03:55:455	other_sales = data['Other_Sales'].sum()
10:03:55:455	global_sales = data['Global_Sales'].sum()
10:03:55:455	
10:03:55:455	# Determine the region with the highest sales
10:03:55:455	sales_dict = {'North America': na_sales, 'Europe': eu_sales, 'Japan': jp_sales, 'Other': other_sales, 'Global': global_sales}
10:03:55:455	max_region = max(sales_dict, key=sales_dict.get)
10:03:55:455	max_sales = sales_dict[max_region]
10:03:55:455	
10:03:55:455	# Output the sales results for each region and the region with the highest sales
10:03:55:455	print(sales_dict)
10:03:55:455	print(f'The region with the highest sales is {max_region} with {max_sales} million.')
10:03:55:455	_02_autogen.Plugins.CodeInterpretionPlugin: Debug: (425900d75443a2f7f2f58f05f18d48d31a06372f8bf839fafa1fd29ab9c5632b)# pip install -r /var/app/requirements.txt
***** Logs from pip installation ommitted *****
10:04:09:085	_02_autogen.Plugins.CodeInterpretionPlugin: Debug: (425900d75443a2f7f2f58f05f18d48d31a06372f8bf839fafa1fd29ab9c5632b)# python /var/app/code.py
10:04:09:584	_02_autogen.Plugins.CodeInterpretionPlugin: Debug: (425900d75443a2f7f2f58f05f18d48d31a06372f8bf839fafa1fd29ab9c5632b): /var/app/code.py:1: DeprecationWarning: 
10:04:09:584	Pyarrow will become a required dependency of pandas in the next major release of pandas (pandas 3.0),
10:04:09:584	(to allow more performant data types, such as the Arrow string type, and better interoperability with other libraries)
10:04:09:584	but was not found to be installed on your system.
10:04:09:584	If this would cause problems for you,
10:04:09:584	please provide us feedback at https://github.com/pandas-dev/pandas/issues/54466
10:04:09:584	        
10:04:09:584	  import pandas as pd
10:04:09:584	{'North America': 4392.950000000001, 'Europe': 2434.1299999999997, 'Japan': 1291.0200000000002, 'Other': 797.7500000000001, 'Global': 8920.44}
10:04:09:584	The region with the highest sales is Global with 8920.44 million.
```

#### Plot charts from the file content

You can also ask the copilot to plot some charts: 

```
From the vgsales.csv file, plot a chart showing the distribution of sales by region.
```