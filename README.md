Eagle
=====

Eagle is a simple Windows utility that runs from the system tray and monitors directories you specified on the settings. It can also send notification emails. Pulling TFS checking history is pending...

##More Info

I needed a quick application that would get out of my way and work in the background. I wanted to monitor directories and email/log when a something changed on such dirs. The current project I'm working on, we are using TFS and manual deployment (copying files from our PCs to the servers...yeah, I know!). The problem is that we lose track of who deployed what and we our remote devs...anyways...I am on vacation and wanted to have some fun.

This is a work in progress. The initial code was written and made functional on 3 hours. I had paid little attention to design or architecture. This is a very small project mostly for academic purposes.

##Tools

Eagle is being developed using .NET 4.5 and WPF. Obviously, using Visual Studio 2012.

##TODO

 - Refactor to use MVVM and write some Unit Test
 - Add Source Control integration, so dirs monitored can be associated with a repo and notifications can include the last 5 checkings for instance
 - I don't know...whatever else I can come up with...
 
##But Why? 
 
This a small utility that I put together very quickly solving a specific problem I'm having. I don't expect this will be useful for anybody else. It is also possible that a similar tool exists with better capabilities. I spent no time finding that out, I jumped to coding at once. 
 


