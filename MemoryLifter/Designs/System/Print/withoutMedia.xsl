<?xml version='1.0' encoding='utf-16'?>
<!--  MemoryLifter Printing Stylesheet   -->
<!--  Version 1.00   -->
<!--  (c) 2006 LearnLift   -->
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:Status="urn:status" exclude-result-prefixes="Status"
                xmlns:CardObject="urn:cardobject"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:scripts="urn:my-scripts">

  <xsl:output method="xml" version="1.0"
              doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
              doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" indent="yes" omit-xml-declaration="no" />

  <xsl:param name="answercaption" />
  <xsl:param name="questioncaption" />
  <xsl:param name="description" />
  <xsl:param name="chapters" />
  <xsl:param name="baseURL" />
  <xsl:param name="titleText" />

  <xsl:template match = "dictionary">
    <html>
      <head>
        <title>
          <xsl:value-of select="$titleText"/>
        </title>
        <base href="{$baseURL}" />
        <style type="text/css">
          <![CDATA[
        body
	    { color: #000000;
	    background-color: #ffffff;  
	    font-family: Helvetica, Arial;
	    text-align: center;
		}
    
	a { color: dark-blue;
	  text-decoration: underline; 
	  }

	.mlquestion { 
		color: #ff0000;
		font-family: Helvetica, Arial;
		font-size: 150%;
		font-style: normal;
		font-weight: normal;
	  	}

	.mlanswer { 
		color: #ff0000;
		font-family: Helvetica, Arial;
		font-size: 150%;
		font-style: normal;
		font-weight: normal;
	  	}

      div.tableContainer {
       width: 99%;		/* table width will be 99% of this*/
	    height: 500px; 	/* must be greater than tbody*/
	    overflow: auto;     /* tells browser to scroll if contents do not fit */
	    margin: 0 auto;
        }

        table {
	    width: 99%;		/*100% of container produces horiz. scroll in Mozilla*/
	}
	
        table>tbody	{  /* child selector syntax which IE6 and older do not support*/
	    overflow: auto; 
	    height: 235px;
	    overflow-x: hidden;
	}
	
        thead td	{
	    font-size: 14px;
	    font-weight: bold;
	    text-align: center;
	    color: steelblue;
	    position: relative; 
	    top: expression(document.getElementById("data").scrollTop-2); /*IE5+ only*/
	}
        
        tr      {
            page-break-inside: avoid;
        }
	
        td	{
	    color: #000;
	    padding-right: 2px;
	    font-size: 12px;
	    text-align: center;
            vertical-align:text-top;
	    font-family: Arial,sans-serif;
	    border: solid 1px black;
	}

        tfoot td	{
	    text-align: center;
	    font-size: 11px;
	    font-weight: bold;
	    color: steelblue;
	}
  td:last-child {padding-right: 20px;} /*prevent Mozilla scrollbar from hiding cell content*/
]]>
        </style>

        <!-- print style sheet -->
        <style type="text/css" media="print">
          div.tableContainer
          {
          overflow: visible;
          }
          table>tbody
          {
          overflow: visible;
          }
          td
          {
          height: 14pt;
          }
          thead td
          {
          font-size: 11pt;
          }
          tfoot td
          {
          text-align: center;
          font-size: 9pt;
          border-bottom: solid 1px slategray;
          }
          thead
          {
          display: table-header-group;
          }
          tfoot
          {
          display: table-footer-group;
          }
          thead th, thead td
          {
          position: static;
          }
        </style>
      </head>
      <body>
        <div id="container">
          <div class="tableContainer" id="data">
            <table cellspacing="1" cellpadding="0">
              <thead>
                <tr>
                  <td width="49%%">
                    <xsl:value-of select="$answercaption"/>
                  </td>
                  <td width="49%">
                    <xsl:value-of select="$questioncaption"/>
                  </td>
                </tr>
              </thead>
              <tfoot>
                <tr>
                  <td colspan="5">
                    <xsl:value-of select="$description"/>
                  </td>
                </tr>
              </tfoot>
              <tbody>
                <xsl:apply-templates select="card"/>
              </tbody>
            </table>

          </div>
        </div>
        <!-- end container -->
      </body>
    </html>
  </xsl:template>

  <!-- ***************************** -->
  <!-- * start template definition * -->
  <!-- ***************************** -->

  <xsl:template match = "card">
    <xsl:if test="Status:SendStatus()" />
    <tr>
      <td>
        <xsl:apply-templates select = "questionimage"/>
        <p dir="{CardObject:GetTextDirection('question')}">
          <xsl:apply-templates select = "question"/>
          <xsl:apply-templates select = "questionexample"/>
        </p>
      </td>
      <td>
        <xsl:apply-templates select = "answerimage"/>
        <p dir="{CardObject:GetTextDirection('answer')}">
          <xsl:apply-templates select = "answer"/>
          <xsl:apply-templates select = "answerexample"/>
        </p>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match = "question">
    <font class="mlquestion">
      <xsl:call-template name="format-output">
        <xsl:with-param name="text" select="."/>
      </xsl:call-template>
    </font>
  </xsl:template>

  <xsl:template match = "answer">
    <font class="mlanswer">
      <xsl:call-template name="format-output">
        <xsl:with-param name="text" select="."/>
      </xsl:call-template>
    </font>
  </xsl:template>

  <xsl:template match = "answerimage">
  </xsl:template>

  <xsl:template match = "questionimage">
  </xsl:template>


  <xsl:template match = "answerexample">
  </xsl:template>

  <xsl:template match = "questionexample">
  </xsl:template>

  <xsl:template name="format-output">
    <xsl:param name="text"/>

    <xsl:variable name="from" select="'&quot;, &quot;'"/>

    <xsl:choose>
      <xsl:when test="contains($text, $from)">

        <xsl:variable name="before" select="substring-before($text, $from)"/>
        <xsl:variable name="after" select="substring-after($text, $from)"/>

        <xsl:value-of select="translate($before,'&quot;','')"/>
        <br />
        <xsl:call-template name="format-output">
          <xsl:with-param name="text" select="$after"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="translate($text,'&quot;','')"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>



