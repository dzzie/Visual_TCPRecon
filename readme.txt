
This is a visual tcp reconstruction tool for pcaps.

Load a pcap, it will parse it into communication streams
each host:port pair gets it own node. Under this node each
leg of the communication will get its own sub node. 

each stream is dumped to a single binary file and each block of
communication can be highlighted or extracted by clicking on a sub node.

This application also supports a scripting interface, so that you can
walk the nodes, and extract the data from an external C# script. A sample
script is provided to dump all POST requests to a single text file. If you
create more handy scripts, please submit them for inclusion in the distro!

The tool is very handy for looking at binary communication protocols, and has some
extra tools for web requests.

it also auto extracts all:
   dns requests
   unique ip addresses
   http web requests

the hexed.ocx control will have to be registered on your system using the following 
command:

regsvr32 hexed.ocx

Another project i stumbled across while looking for dns parsing code 
is Network Miner. Its open source and written in C#. Looks like its worth
checking out too:

http://sourceforge.net/projects/networkminer/files/networkminer/NetworkMiner-1.5/


Base project:
 
 tcpRecon: TCP Session Reconstruction Tool  
 author  : Saar Yahalom, 21 Sep 2007 
 original: http://www.codeproject.com/Articles/20501/TCP-Session-Reconstruction-Tool
 license : http://www.codeproject.com/info/cpol10.aspx

 dependancies: (included) make sure to run the cmd "regsvr hexed.ocx" 
 (also installed with pdfstreamdumper/sysanalyzer)

 Tamir.IPLib.SharpPcap.dll which contains:
  PacketDotNet http://sourceforge.net/apps/mediawiki/packetnet/index.php?title=Main_Page
  SharpPcap    http://sourceforge.net/apps/mediawiki/sharppcap/index.php?title=Main_Page

 hexed.ocx -         https://github.com/dzzie/hexed
 winpcap             http://www.winpcap.org/
 


