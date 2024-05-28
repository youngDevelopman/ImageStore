# Updating the comment count

The system needs to align with the following requirements:

- Being able to retrieve a list of posts sorted by the number of comments in descending order along with a number of comments. 
- Maximum response time for any API call except uploading image files - 50 ms

Based on these requirements, the conventional approach of running aggregate functions such as `GROUP BY` and `COUNT` for the comments would be hurting performance a lot, so we would likely not meet the second requirement in the list.

Thus, it was decided to store a number of comments in the *Post* table.

![image](https://github.com/youngDevelopman/ImageStore/assets/31933374/51f1dd51-6877-4775-8fda-1f23de6b7b5a)



## Lost update

Imagine if two persons are trying to comment on the post at the same time. We would likely face the problem which is called lost update.

![image](https://github.com/youngDevelopman/ImageStore/assets/31933374/b66e6f59-49a2-43fc-9b52-93a7600a7992)


## Ways of updating comment count

Now, we need to discover a way of making the comment count column synchronized with an actual amount of comments associated with a particular post
Generally, I have considered three ways of achieving this:
- Using db isolation levels such as repeatable read or serializable
  - The problem with this approach is while the comment is being added the transaction fully locks the post record, so other users who try to read/update the post or write the comments would be blocked. And in case of the high number of reads per second, it might crucially impact the performance
- Using DB trigger
  - Generally, the problem with the records locking is the same here, but the comment count would be updated at the end of the transaction
- Use row version and re-tries *(implemented approach)*
   - EF Core implements optimistic concurrency, which assumes that concurrency conflicts are relatively rare. In contrast to pessimistic approaches - which lock data up-front and only then proceed to modify it - optimistic concurrency takes no locks but arranges for the data modification to fail on save if the data has changed since it was queried. This concurrency failure is reported to the application, which deals with it accordingly, possibly by retrying the entire operation on the new data.
 

Implementation for adding a comment: https://github.com/youngDevelopman/ImageStore/blob/master/src/ImageStore.Application/Comments/Commands/AddComment/AddCommentCommandHandler.cs#L24

Implementation for removing a comment: https://github.com/youngDevelopman/ImageStore/blob/master/src/ImageStore.Application/Comments/Commands/DeleteComment/DeleteCommentCommandHandler.cs


