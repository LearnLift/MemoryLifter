'------------------------------------------------------------------------------------------------
'
' Usage: prepare-stick-config <--target|-t outputpath>
'               <--project|-p projectname>
'               [--help|-?]
'------------------------------------------------------------------------------------------------

' Force explicit declaration of all variables.
Option Explicit

'On Error Resume Next

Dim oArgs, ArgNum
Dim buildPath, projectName, langArray, debugMode
Dim verbose

verbose = false
debugMode = false

Set oArgs = WScript.Arguments
ArgNum = 0
While ArgNum < oArgs.Count

	Select Case LCase(oArgs(ArgNum))
		Case "--target","-t":
			ArgNum = ArgNum + 1
			buildPath = oArgs(ArgNum)
		Case "--project","-p":
			ArgNum = ArgNum + 1
			projectName = oArgs(ArgNum)
		Case "--help","-?":
			Call DisplayUsage
		Case Else:
			Call DisplayUsage
	End Select	

	ArgNum = ArgNum + 1
Wend

If (buildPath = "") Or (projectName = "") Then
	Call DisplayUsage
Else
    Call PrepareConfig
End If

Sub PrepareConfig
	Dim fso, xmlDoc, xnDicDir, xnFirstUse, xnRecetFiles, xnForce, configPath, userConfigPath
	If Not debugMode Then
	    Set fso = CreateObject("Scripting.FileSystemObject")
        configPath = fso.BuildPath(buildPath, projectName & ".exe.config")
        Set xmlDoc = CreateObject("Microsoft.XMLDOM")
        xmlDoc.async = False
        xmlDoc.Load configPath
        Set xnFirstUse = xmlDoc.selectSingleNode("//setting[@name='FirstUse']/value")
        xnFirstUse.Text = "False"
        Set xnForce = xmlDoc.selectSingleNode("//setting[@name='ForceOnStickMode']/value")
        xnForce.Text = "True"
        Set xnDicDir = xmlDoc.selectSingleNode("//setting[@name='DicDir']/value")
        xnDicDir.Text = "MEMSTICK:\LearningModules"
        xmlDoc.Save configPath
        Set fso = Nothing
	End If
End Sub

Sub DisplayUsage
	WScript.Echo "Usage: prepare-stick-config <--target|-t outputpath>"
	WScript.Echo "               <--project|-p projectname>"
 	WScript.Echo "               [--help|-?]"
	WScript.Quit (1)
End Sub

Sub Display(Msg)
	WScript.Echo Now & ". Error Code: " & Hex(Err) & " - " & Msg
End Sub

Sub Trace(Msg)
	if verbose = true then
		WScript.Echo Now & " : " & Msg	
	end if
End Sub
