
This is a visual tcp reconstruction tool for pcaps.

Load a pcap, it will parse it into communicaiton streams
each host:port pair gets it own node. Under this node each
leg of the communication will get its own sub node. 

it should handle packet reassembly and fragments ok, 
reordering the packets and conglomerating the data into
the proper chunks. 

I didnt write the reassembly code, just the UI display and extraction.
credits are at the bottom of the file.

each stream is dumped to a single binary file. 

When you click on a subnode, it will highlight that section of the stream 
in the hexeditor on the right. You can right click on the hexeditor pane and
save the section as.. or you can right click on the parent stream in the treeview
on hte left and choose to extract all streams. They will be dumped to the folder
you specify with incremental filenames.

this iis just a quick project, built on top of a mountain of code by other 
authors to play with some ideas about pcap parsing. 

the hexed.ocx control will have to be registered on your system using the following 
command:

regsvr hexed.ocx

subnodes label format is: file_start_offset,data_length  sourceport:


/*
 *  This code was modified by dzzie@yahoo.com from the base at:
 *  
 *  TCP Session Reconstruction Tool  
 *  author  : Saar Yahalom, 21 Sep 2007 
 *  original: http://www.codeproject.com/Articles/20501/TCP-Session-Reconstruction-Tool
 *  license : http://www.codeproject.com/info/cpol10.aspx
 * 
 *  dependancies: (included) make sure to run the cmd "regsvr hexed.ocx"
 *     Tamir.IPLib.SharpPcap.dll which contains:
 *          PacketDotNet   http://sourceforge.net/apps/mediawiki/packetnet/index.php?title=Main_Page
 *          SharpPcap      http://sourceforge.net/apps/mediawiki/sharppcap/index.php?title=Main_Page
 *     hexed.ocx -         https://github.com/dzzie/hexed
 *     managedLibnids.dll  http://www.codeproject.com/KB/IP/TcpRecon/Libnids-119_With_managedLibnids.zip
 *     winpcap             http://www.winpcap.org/
 *  
 */

