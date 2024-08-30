# Rss Feed Aggregator
This is a project to showcase FastEndpoints and a minimal Vertical Slice Architecture.  
This repository is to accompany this youtube video: https://youtu.be/-OWj0MrjYMo

If you're coming here from [breakneck](https://breakneck.dev), this will give you a good idea of the style.

This project manages RSS feeds from blogs. You can add certain blogs to the system and then users can subscribe to those blogs.  
The system constantly synchronizes the newest posts from all blogs.

## Features
- Register as a user and get an api token
- Login with your api token and get a jwt token
- Create RSS feeds from blogs as an admin
- Show all RSS feeds in the system
- Subcribe and unsubscribe to RSS feeds
- Show users the newest posts of followed RSS feeds
- Synchronize the newest posts from all RSS feeds in the system every minute concurrently in the background

## Technical
- AspNetCore Backend API
- Authentication / Authorization with JWT
- Entity Framework Core to store users, feeds and posts
- HangFire to synchronize feeds in the background concurrently 
- Request Response Architecture

## Getting Started
These are some rss feeds you can play around with: 
- https://blog.boot.dev/index.xml
- https://wagslane.dev/index.xml
- https://www.hanselman.com/blog/feed/rss
