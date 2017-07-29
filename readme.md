#ConvertMKVToMP4
===============

1. The tool crawls the path you pass it and converts every mkv file it encounters into an mp4 file using ffmpeg.

2. The tool starts a filesystemwatcher for the path you pass as a parameter and converts every mkv file it encounters into an mp4 file using ffmpeg.

Usage:
-----

Create a batch file with the following content: 

    c:\tools\ConvertMKVToMP4.exe "c:\media"

Run the batch file.

By default, the original file will be deleted. If you whish to override this behaviour, pass false as the second parameter like so:

    c:\tools\ConvertMKVToMP4.exe "c:\media" false


