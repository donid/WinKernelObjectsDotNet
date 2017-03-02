# WinKernelObjectsDotNet
A .Net library that allows to query information about Windows kernel objects
 (can be use to find the process that is locking a file or serial-port)


I needed a tool that can tell me which process is locking a certain serial port.
 After some research, I found that this is basically the same problem as finding 
 the process which is locking a file in the file-system. I also discovered, that there 
 are many requests in bulletin boards how to achieve this in .Net. It seems that
 the answers largely consist of very similar code snippets that are copied from
 one post to another.


This clearly showed me the need for an open source library where development and bugfixing
can be done in an organized way.

An alternative method to find the 'locking' process is to use the 'Restart Manager' that is available
in modern Windows versions, but that doesn't seem to work for serial ports. So I
used the NtQuerySystemInformation API, which also has some drawbacks:

- some information can only be retrieved with a kernel driver
- the API is partly undocumented
- some API calls might hang infinitely for certain objects/handles


Although I had specific requirements, I wanted the library to be more broadly useable.
 So I created an API that is a bit more complex, but if you are just interested in finding
 the 'locking' process you can use this simple code:

 Process[] lockingProcesses = new KernelObjectsFacade().GetProcessesUsingPath(@"c:\somefolder\somefile.txt");

This also works if you specifiy e.g. 'COM1' instead of a file-path.


Additionally I have created a GUI that allows to display all handles of the running OS,
 but it uses the XtraGrid Component from DevExpress which has a commercial license.


 The library and GUI has been tested with Windows 10 64bit with and without the 'Prefer 32bit' flag activated.
