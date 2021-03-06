<?xml version='1.0' encoding='utf-16'?>
<!--  (c) 2006 LearnLift   -->
<!--  MemoryLifter Stylesheet   -->
<!--  Version 2.0 Date: 2007-10-08   -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:CardObject="urn:cardobject">

  <xsl:template name="displayImagemode">
    <xsl:choose>
      <xsl:when test="$side = 'question'">
        <div>
          <xsl:call-template name="displayImage" />
        </div>
        <div>
          <xsl:if test="CardObject:ContainsAudio($side)">
            <xsl:value-of select="$clickForQuestion"/>
            <xsl:call-template name="displayAudio" />
          </xsl:if>
          <xsl:call-template name="displayVideo" />
        </div>
        <div>
          <xsl:if test="CardObject:ContainsExampleAudio($side)">
            <xsl:value-of select="$clickForExample"/>
            <xsl:call-template name="displayExampleAudio" />
          </xsl:if>
        </div>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="displayWordmode" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>