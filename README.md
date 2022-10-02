# ImageGram
A prototype of a system for uploading images and commenting on them.

## General Description
The system consists of several Azure Functions that serve as separated services for different functionalities, such as:

### 1. Post Service
A HttpTrigger function serve as the endpoint to create or save a new post. In this function, Post dat awill be saved into a Azure Cosmos DB and the attached image file will be uploaded to the Azure Blob/Storage Account.

### 2. Convert Image Service
A Storage/Blob trigger function to handle the image conversion. In summary, after the original file has been successfully uploaded to the Blob storage, this Azure Function will convert the original image and upload it to another Blob storage.

### 3. Comment Service
A HttpTrigger to serve as an endpoint to save and delete a comment. The deletion in this service is a soft delete one, therefore it needs another worker service or scheduler to permanently delete the comment from the database.

### 4. NewsFeed Aggregator Service
A Cosmos DB trigger function responsible to aggregate the Post and Comment. Every time there is an update in the Post and/or Comment database, this function will pick up the latest data and use it to create or update the NewsFeed data.

### 5. NewsFeed Service
Another HttpTrigger service that can be used to fetch or retrieve the NewsFeed data. The NewsFeed could be filtered by a datetime indicating the latest seen post date.
