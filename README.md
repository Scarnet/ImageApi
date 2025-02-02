### Overview
- This project for a coding challenge for Fullstack Developer Exercise from WECHEER.IO
- In the serverless.template two AWS Lambdas Are defined one that is exposed through an API gateway and another one that is integrated with Kinesis stream provider

### Add new event
- Call POST https://b8m0yrqzeg.execute-api.eu-central-1.amazonaws.com/prod/image
- 
### Sample payload
{
  "ImageUrl": "https://www.huronelginwater.ca/app/uploads/2019/03/test.jpg",
  "Description": "Test new image"
}

### Get latest event
- Call GET https://b8m0yrqzeg.execute-api.eu-central-1.amazonaws.com/prod/events

### Sample response
{
    "lastHourCount": 1,
    "latestEvent": {
        "imageUrl": "https://www.huronelginwater.ca/app/uploads/2019/03/test.jpg",
        "description": "Test new image",
        "timestamp": "2025-02-02T20:16:46.97734Z"
    }
}

### Frontend application
Hosted in S3 bucket and can be accessed here http://image-viewer-test.s3-website.eu-central-1.amazonaws.com/display

### Note
Although a Kinesis streamer is defined, due to time limit, a consumer was not implemented
