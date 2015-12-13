
About:
------------------------------------
This is a visual tcp reconstruction tool for pcaps.

The way I generally use this is as follows:

1) take a packet capture with wireshark
2) filter the pcap to hosts of interest to get decent size
3) save as a classic .pcap file 
4) load in visual_tcprecon for data exploration and file extraction

When you load a pcap, it will parse it into communication streams
each host:port pair gets it own node. Under this node each
leg of the communication will get its own sub node. 

each stream is dumped to a single binary file and each block of
communication can be highlighted or extracted by clicking on a sub node.

The tool is very handy for looking at binary communication protocols, and has some
extra tools for web requests.

it also auto extracts all:
   dns requests
   unique ip addresses
   http web requests

<img src="https://raw.githubusercontent.com/dzzie/Visual_TCPRecon/master/tcprecon_screenshot.png">


Scripting capabilities:
------------------------------------
This application also supports a scripting interface, so that you can
walk the nodes, and extract the data from an external C# script. Sample
scripts are provided in the ./Visual_TCPRecon/Scripts/ folder. 

If you create more handy scripts, please submit them for inclusion in the distro!


Dependancies:
------------------------------------
This app has been compiled to run as a 32bit exe. This is required because it uses
the 32bit hexed.ocx ActiveX control. On startup the application will detect if this
ocx has been registered on your system yet or not. If not, it will automatically
register it for you and then should be able to load seamlessly.


Other work:
------------------------------------
Another project i stumbled across while looking for dns parsing code 
is Network Miner. Its open source and written in C#. Looks like its worth
checking out too:

http://sourceforge.net/projects/networkminer/files/networkminer/NetworkMiner-1.5/


Credits:
------------------------------------
 tcpRecon: TCP Session Reconstruction Tool  
 author  : Saar Yahalom, 21 Sep 2007 
 original: http://www.codeproject.com/Articles/20501/TCP-Session-Reconstruction-Tool
 license : http://www.codeproject.com/info/cpol10.aspx

 dependancies: (included) make sure to run the cmd "regsvr hexed.ocx" 
 (also installed with pdfstreamdumper/sysanalyzer)

  PacketDotNet http://sourceforge.net/apps/mediawiki/packetnet/index.php?title=Main_Page
  SharpPcap    http://sourceforge.net/apps/mediawiki/sharppcap/index.php?title=Main_Page

  hexed.ocx -         https://github.com/dzzie/hexed
  winpcap             http://www.winpcap.org/
  whois.exe -         Freeware provided courtesy of NirSoft

  ReassemblePacket was updated 6.17.2015 with code from Mark Woan's SessionViewer
  https://github.com/woanware/SessionViewer  


Known Bugs:
------------------------------------
Sometimes I still get errors parsing packets back into continious data streams. The
bug affects both the original stream reassembly code, as well as the updated logic
from Woans sessionviewer. It does not happen very often so I am ignoring it for now
since that is some pretty complicated logic in there to tinker with.
