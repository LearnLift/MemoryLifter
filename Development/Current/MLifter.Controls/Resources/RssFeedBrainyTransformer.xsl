<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:user="http://www.memorylifter.com/xslt/scripts/"
                exclude-result-prefixes="user">
  <xsl:output encoding="iso-8859-1" omit-xml-declaration="yes" method="html"/>
  <!-- Number of news - default value set to for. -->
  <xsl:param name="displayNumber" select="4" />
  <xsl:variable name="count" select="count(//item)" />
  <xsl:variable name="selected" select="user:GetRandom($count)" />

  <msxsl:script implements-prefix="user" language="C#">
    <![CDATA[
    public string ConvertDate(string dateStr)
    {
      try
      {
        DateTime dt = DateTime.Parse(dateStr);
        return dt.ToString("MMM dd, yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
      }
      catch
      {
        return dateStr;
      }
    }
    public string CleanTypo3String(string typo3Str)
    {
      return typo3Str.Replace("&;", "");
    }
    public int GetRandom(int count)
    {
      Random rand = new Random((int)DateTime.Now.Ticks);
      return rand.Next(1, count);
    }
    ]]>
  </msxsl:script>
  
  <xsl:template match="/">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="http://www.memorylifter.com/fileadmin/styles/brainy-newsfeed.css" />
      </head>
      <body leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
        <div class="mlnews">
          <div class="news-latest-container">
            <xsl:apply-templates select="/rss/channel/item" />
          </div>
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="item">
    <xsl:if test="position() &gt;= $selected and position() &lt;= $selected">
      <div class="news-latest-item">
        <span class="news-latest-morelink" style="float:right;">
          <a href="{link/text()}" title="{title/text()}" target="_blank">[more]</a>
        </span>
        <h3>
          <a href="{link/text()}" title="{title/text()}" target="_blank">
            <xsl:value-of select="title/text()"/>
          </a>
        </h3>
        <a href="{link/text()}" title="{title/text()}" target="_blank"></a>
        <p class="bodytext">
          <xsl:value-of select="normalize-space(user:CleanTypo3String(description/text()))" disable-output-escaping="yes"/>
        </p>
        <xsl:if test="(position() &lt; $displayNumber) and (position() &lt; last())">
          <hr class="clearer" />
        </xsl:if>
      </div>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>