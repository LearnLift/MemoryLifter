<?xml version='1.0' encoding='utf-16'?>
<!--  (c) 2006 LearnLift   -->
<!--  MemoryLifter Stylesheet   -->
<!--  Version 2.0 Date: 2007-10-08   -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template name="defaultstyle">
	  <xsl:text disable-output-escaping="yes">
<![CDATA[
body
{
	color: #000000;
	background-color: #ffffff;
	font-family: Helvetica,Arial;
	text-align: center;
	font-size: 18pt;
}

a
{
	text-decoration: underline;
}

a:visited
{
	color: #000000;
}

button
{
	vertical-align: middle;
	cursor: pointer;
}

div.hide
{
	display: none;
}

div.video
{
	margin: 20px 0px;
	display: none;
}

div.imageSubtitle
{
	font-size: 50%;
}

#questionHeader
{
	padding: 5px;
	margin-bottom: 15px;
	min-height: 30px;
}

#questionHeader img
{
	margin-left: 10px;
}
#questionHeader:hover
{
	background-color: #ddeeff;
}

#questionExample
{
	padding: 5px;
	margin-bottom: 15px;
	min-height: 30px;
}

#questionExample img
{
	margin-left: 10px;
}

#questionExample:hover
{
	background-color: #ddeeff;
}

.question
{
	color: #353a90;
}

.questionWord
{
}

.questionExample
{
	font-size: 80%;
	font-style: italic;
	font-weight: normal;
	text-align: center;
}

#answerHeader
{
	padding: 5px;
	margin-bottom: 15px;
	min-height: 30px;
}

#answerHeader img
{
	float: right;
	margin-left: 10px;
}

#answerHeader:hover
{
	background-color: #778899;
}

#answerExample
{
	padding: 5px;
	margin-bottom: 15px;
	min-height: 30px;
}

#answerExample img
{
	float: right;
	margin-left: 10px;
}

#answerExample:hover
{
	background-color: #778899;
}

td.answer
{
	background-image: none;
	text-align: center;
}

.answer
{
	color: #353a90;
}

.answerWord
{
}

.answerExample
{
	font-size: 80%;
	font-style: italic;
	font-weight: normal;
	text-align: center;
}

.answerCorrect
{
	background-color:	#9CC602;
}

.answerWrong
{
	background-color:	#E00D06;
}

.image
{
	cursor: pointer;
}

.buttonPlayAudio
{
	margin: 0px;
	background-color: transparent;
	padding: 0px;
	border: 0px;
}

.buttonPlayExampleAudio
{
	margin: 0px;
	background-color: transparent;
	padding: 0px;
	border: 0;
}

.buttonPlayVideo
{
	margin: 0px;
	margin-top: 15px;
	background-color: transparent;
	padding: 0px;
	border: 0;
}

#userInputBox
{
	margin-bottom: 15px;
	height: expression( this.scrollHeight > 160 ? "160px" : "auto" );
	/*max-height: 160px;*/
	overflow: auto;
	overflow-y: auto;
	overflow-x: hidden;
}

#main
{
}

#feedback
{
	width: 85%;
	position: absolute;
	overflow: hidden;
	text-align: left;
	margin: 0px;
	font-size: 14pt;
	left: 7.5%;
	_left: 9%;  /* this is only applies to IE6 */
	bottom: 7.5%;
	border: 1px solid #000000;
}

.feedbackCorrect { background-color: #9CC602; }

.feedbackWrong { background-color: #E00D06; }

#feedback table
{
	width: 100%;
	overflow: hidden;
	font-size: 14pt;
	font-weight: normal;
	color: #000000;
}

#feedback table td
{
	vertical-align: middle;
}

#feedbackMessage
{
	height: expression( this.scrollHeight > 160 ? "160px" : "auto" );
	/*max-height: 160px;*/
	overflow: auto;
	overflow-y: auto;
	overflow-x: hidden;
	margin-bottom: 15px;
}

#youEnteredText { font-style: italic; }

#correctAnswerText { font-style: italic; }

#boxMessage
{
	width: 100%;
	text-align: center;
	font-size: 11pt;
  font-weight: normal;
	padding: 0 0 5px 0;
}

#closeBar
{
	height: 20px;
/*
	border-bottom: 1px solid #ffffff;
	background-color: #cccccc; */
}

.closebarCorrect { background-color: #ffffff; }

.closebarWrong { background-color: #000000; }

#closeBar button
{
	float: right;
	height: 16px;
	width: 16px;
	margin: 2px;
	background-color: transparent;
	padding: 0px;
	border: 0px;
}

.correctInput
{
	color: #9CC602;
}

.wrongInput
{
	color: #E00D06;
}

.correctAnswer
{
	color: #9CC602;
}

.wrongAnswer
{
	color: #E00D06;
}

#buttonMirror
{
	float: left;
	height: 40px;
	width: 40px;
	margin-right: 10px;
}

#listeningModeImage #image
{
  margin: 10px 0 0 0;
  display: none;
  padding: 0;
}

#buttonListeningMode button
{
  margin: 10px 0 0 0;
  text-weigth: bold;
  text-align: center;
  vertical-align: middle;
}
]]></xsl:text>
  </xsl:template>
</xsl:stylesheet>