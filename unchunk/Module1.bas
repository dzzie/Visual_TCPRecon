Attribute VB_Name = "Module1"
Sub Main()

    On Error Resume Next
    Dim d As String
    
    Set o = CreateObject("vbdevkit.CFileSystem2")
    If o Is Nothing Then
        MsgBox "please install PDF stream dumper to get dependancies. (hexed.ocx and vbdevkit.dll)", vbInformation
        End
    Else
        
        Load Form1
        Form1.Visible = True
        
        If Len(Command) > 0 Then
            d = Replace(Command, """", Empty)
            If FileExists(d) Then
                Form1.Text1 = d
                Form1.Command2_Click
            ElseIf FolderExists(d) Then
                Form1.defPath = d
            End If
        End If
 
    End If
    
End Sub



Function FolderExists(path) As Boolean
  If Dir(path, vbDirectory) <> "" Then FolderExists = True _
  Else FolderExists = False
End Function

Function FileExists(path) As Boolean
  If Dir(path, vbHidden Or vbNormal Or vbReadOnly Or vbSystem) <> "" Then FileExists = True _
  Else FileExists = False
End Function


