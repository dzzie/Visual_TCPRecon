VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   3765
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   8310
   LinkTopic       =   "Form1"
   ScaleHeight     =   3765
   ScaleWidth      =   8310
   StartUpPosition =   2  'CenterScreen
   Begin VB.ListBox List1 
      Height          =   2985
      Left            =   90
      TabIndex        =   4
      Top             =   630
      Width           =   8070
   End
   Begin VB.CommandButton Command2 
      Caption         =   "Unchunk"
      Height          =   330
      Left            =   7020
      TabIndex        =   3
      Top             =   90
      Width           =   1140
   End
   Begin VB.CommandButton Command1 
      Caption         =   "..."
      Height          =   330
      Left            =   5985
      TabIndex        =   2
      Top             =   90
      Width           =   780
   End
   Begin VB.TextBox Text1 
      Height          =   330
      Left            =   585
      TabIndex        =   1
      Top             =   90
      Width           =   5190
   End
   Begin VB.Label Label1 
      Caption         =   "input"
      Height          =   240
      Left            =   90
      TabIndex        =   0
      Top             =   135
      Width           =   555
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Dim dlg As New clsCmnDlg2
Dim fso As New CFileSystem2
Dim chunk As Long

Private Sub Command1_Click()
    Text1 = dlg.OpenDialog(AllFiles)
    If Len(Text1) = 0 Then Exit Sub
End Sub

Private Sub Command2_Click()
    
    chunk = 0
    List1.Clear
    
    If Len(Text1) = 0 Then Exit Sub
    
    If Not fso.FileExists(Text1) Then
        MsgBox "in does not exist"
        Exit Sub
    End If
    
    Dim outfile As String
    Dim b() As Byte
    Dim f As Long
    Dim f2 As Long
    Dim sz As Long
    
    Dim isChunked As Boolean
    Dim hsz As Long
    
    On Error GoTo hell
    
    outfile = fso.GetParentFolder(Text1) & "\" & fso.FileNameFromPath(Text1) & ".gzip"
    If inFile = outfile Then
        MsgBox "infile = default outfile name?"
        Exit Sub
    End If
        
    If fso.FileExists(outfile) Then Kill outfile
    
    If hasHttpHeader(Text1, isChunked, hsz) Then
        List1.AddItem "found http header.."
        If Not isChunked Then
            List1.AddItem "aborting could not find Transfer-Encoding: chunked"
            Exit Sub
        End If
        List1.AddItem "header size is " & Hex(hsz)
    Else
        List1.AddItem "no http header detected..assuming starts with 0d0a[chunk][0d0a]"
    End If
    
    f = FreeFile
    Open Text1 For Binary As f
    If hsz > 0 Then Seek f, hsz - 1
    
    f2 = FreeFile
    Open outfile For Binary As f2

    Do While Not EOF(f)
        sz = chunkSize(f)
        If sz = 0 Then Exit Do
        ReDim b(sz - 1) '0 based...
        Get f, , b()
        Put f2, , b()
        List1.AddItem "start: " & Hex(b(0)) & " end: " & Hex(b(UBound(b)))
    Loop
        
    Close f
    Close f2
    
    List1.AddItem "complete no errors"
    
    Exit Sub
hell:
    
    List1.AddItem "aborting with errors:" & Err.Description
End Sub

Function hasHttpHeader(inFile As String, ByRef isChunked As Boolean, ByRef hsz As Long) As Boolean
    Dim a As Long
    Dim b As Long
    Dim dat As String
    
    dat = fso.ReadFile(inFile)
    If VBA.Left(dat, 4) = "HTTP" Then hasHttpHeader = True Else Exit Function
    
    a = InStr(1, dat, vbCrLf & vbCrLf)
    If a > 0 Then
        hsz = a + 2
        dat = Mid(dat, 1, hsz)
        If InStr(1, dat, "Transfer-Encoding: chunked", vbTextCompare) > 0 Then isChunked = True
    End If

End Function


Function chunkSize(f As Long) As Long
    Dim b As Byte
    Dim tmp As String
    Dim pos As Long
    
    pos = Seek(f)
    
    '0d0a[hexchunksize]0d0a
    For i = 0 To 8
        Get f, , b
        If b = &HA Or b = &HD Then
            'ignore..
        Else
            If IsHex(b) Then
                tmp = tmp & Chr(b)
            Else
                Exit For
            End If
        End If
    Next
    
    If b <> &HA Or b <> &HD Then
        'Debug.Print "back 1: " & Hex(b)
        Seek f, pos + i
    End If
    
    chunkSize = CLng("&h" & tmp)
    List1.AddItem "chunk " & chunk & " size: " & tmp
        
End Function


Function IsHex(it As Byte) As Boolean
    On Error GoTo out
      IsHex = CBool(InStr(1, "1234567890abcdef", Chr(it), vbTextCompare) > 0)
    Exit Function
out:  IsHex = False
End Function

Sub push(ary, value) 'this modifies parent ary object
    On Error GoTo init
    X = UBound(ary) '<-throws Error If Not initalized
    ReDim Preserve ary(UBound(ary) + 1)
    ary(UBound(ary)) = value
    Exit Sub
init:     ReDim ary(0): ary(0) = value
End Sub


