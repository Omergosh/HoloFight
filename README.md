# HoloFight

A barebones 2D fighting game based on HolyMyth characters from Hololive!

This project started off the intention of making an incredibly simplified 2D fighting game that I could hook up to use GGPO to implement rollback netcode! I first looked at https://github.com/nykwil/UnityGGPO with the intention of utilizing it - which can be some in how I initially structured part of my codebase (e.g. a static Constants class).

For now though, I'm putting all thoughts of implementing netcode on hold! That's something I might consider waaay in the future, but I've gotta make the actual game first.

I drew the art myself and wrote the majority of the code as well (except for the packages I've used and credited), so I figured I might as well make the repo public so people can look at it! It's not like I'll ever make money off of this or anything. Let the world see my shamelessly inefficient C#/Unity prototyping process.

If you have any questions, feel free to message me!

Credits:
- Newtonsoft's Json.NET (https://www.newtonsoft.com/json): I used this package to read/write frame data to and from JSON files! Super helpful for making the Frame Data Editor functional.
- s-m-k's Unity Editor extension (https://github.com/s-m-k/Unity-Animation-Hierarchy-Editor), which is basically just the one .cs file from their public repo. Saved me at least an hour or two of refactoring animations by turning the process into a few button presses.
- Noiseless World (https://noiselessworld.net/en). Instrumental music, free to use. New song every month! They're up on SoundCloud, and even up on Spotify!

Inspirations:
- https://github.com/nykwil/UnityGGPO (initial code direction)
- https://www.youtube.com/watch?v=hzhmXICuahs (huge inspiration "Excuse my Rudeness, but Could You Please RIP? / Desumetal Remix" by Turbo)
