# Back-of-the-envelope estimation

Given the following usage forecast we can make some data storage size estimations:

- Minimum throughput handled by the system - 100 RPS
- 1k uploaded images per 1h = 24k per day
- 100k new comments per 1h = 240k per day


## Calculating requests per day

Let's break down the 100 RPS:

Assume that out of every 100 RPS there are **10 requests for writes** (add comment or post) and **90 requests for reads**

Then, let's assume that out of 10 requests for writes we have **2 requests for image upload** and **8 requests for adding the comments**;

Let's do some math then:

- 100 * 86400 = 8 640 000 *requests per day*;
- 8 640 000 * 0.1 = 864 000 *write request per day*;
- 8 640 000 * 0.9 = 7 776 00 *read requests per day*;
- 864 000 * 0.2 = 172 800 *requests for the image upload per day*;
- 864 000 * 0.8 = 688 000 *requests for the comments upload per day*;

## Calculating the size of the data that needs to be stored

To calculate the number of bytes one record takes in the table, we'll break down each column based on its data type. 
Note that the actual size can vary slightly due to database management system overhead and the actual content for variable-length fields. Here's a rough estimate:

### Post-related data

#### PostRequest table:
| Column | Size |
|--------|--------|
|Id (uniqueidentifier) | 16 bytes|
|Status (int) | 4 bytes |
| FailureDetails (nvarchar(max)) | This is variable length. nvarchar uses 2 bytes per character. The max size can be up to 2^31-1 bytes (around 2 GB), but let's consider a practical example where this is empty or contains 100 characters: 200 bytes + 2 bytes for the length indicator.|
|CreatedAt (datetime2(7))| 8 bytes |
| UpdatedAt (datetime2(7))| 8 bytes |
|Data (nvarchar(max)) | Like FailureDetails, this is variable. Assuming it contains 1000 characters as an example: 2000 bytes + 2 bytes for the length indicator|
|PostId (uniqueidentifier) | This is nullable, but for a non-null value: 16 bytes|
|UserId (uniqueidentifier)| 16 bytes |

**Result: 2272 bytes per record**

#### Post table:
| Column | Size |
|--------|--------|
|Id (uniqueidentifier)| 16 bytes|
|Caption (nvarchar(100))| This is variable up to 200 characters (100 Unicode characters). Since nvarchar uses 2 bytes per character, the maximum is 200 bytes + 2 bytes for the length indicator, but if null, it could be less.|
|Image (nvarchar(max))| Assuming an average URL length of 100 characters, it would be 200 bytes + 2 bytes for the length indicator|
|CreatedAt (datetime2(7))| 8 bytes|
|UpdatedAt (datetime2(7))| 8 bytes |
|CommentsCount (int) | 4 bytes|
| Version (timestamp) | The timestamp data type is a synonym for rowversion, which is automatically generated and unique within the database. It is 8 bytes|
|UserId (uniqueidentifier)| 16 bytes|

**Result:  464 bytes bytes per item**

#### Image size

Let's assume that the average original size image being uploaded to our system is 3MB and the processed image is 1/3 of the original size which is 1 MB.

#### Putting all together

-Database combined size for one post record = 2272 + 464 = 2736 bytes;
-Size of the orginal and processed images = 3 mb + 1mb = 4mb

**Knowing all of that, let's calculate the load per day for post-related data**

- Database load for post creation = 172 800 * 2736 = 472 780 800 bytes per day(~473 MB);
- S3 load for post creation = 172 800 * 4mb = 691 200 mb per day(~692 GB);


### Comments-related data
| Column | Size |
|--------|--------|
|Id (uniqueidentifier)| 16 bytes|
|PostId (uniqueidentifier)| 16 bytes|
|Content (nvarchar(100))| This column can store up to 100 Unicode characters. Since nvarchar uses 2 bytes per character, this means a maximum of 200 bytes + 2 bytes for the length indicator.|
|CreatedAt (datetime2(7))| 8 bytes|
|UpdatedAt (datetime2(7))| 8 bytes|
|UserId (uniqueidentifier)| 16 bytes|

Result: 266 bytes per record

Let's calculate the daily size of the comments stored in db:
- 688 000 * 266 = 183 008 000 bytes per day for storing user comments(~183 MB per day);

## Annual data size estimation
- ~8 760 000 uploaded images per year
- ~87 600 000 uploaded comments per year
- ~172 GB in SQL Database and 272 TB for S3 for image uploading per year
- ~67 GB in SQL Database for storing the comments per year

*Note, that this statistic is approximate. It does not assume the number of people who joined our system, or who have been inactive for quite some time, or daily spikes of the data known as celebrity problem.*
