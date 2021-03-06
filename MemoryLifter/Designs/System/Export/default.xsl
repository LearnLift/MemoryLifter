<?xml version="1.0" encoding="utf-16" ?>
<!--  MemoryLifter Exporing Stylesheet   -->
<!--  Version 1.00   -->
<!--  (c) 2006 LearnLift   -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:Status="urn:status" xmlns:Export="urn:export" exclude-result-prefixes="Status">
	<xsl:output method="text"/>
	<xsl:param name="separator"/>
	<xsl:param name="chapters"/>
	<xsl:param name="header"/>
	<xsl:param name="fieldsElement"/>
	<xsl:param name="copyFiles"/>

	<xsl:template match="/">
		<xsl:value-of select="$header"/>
		<xsl:call-template name="linefeed" />
		<xsl:apply-templates select="//card"/>
	</xsl:template>

	<xsl:template match="card">
		<xsl:variable name="chapterid" select="./chapter"/>
		<xsl:if test="Status:SendStatus()" />
		<xsl:if test="contains($chapters, concat($separator, chapter, $separator))">
			<xsl:call-template name="getFields">
				<xsl:with-param name="theCard" select="."/>
				<xsl:with-param name="chaptername" select="//chapter[@id=$chapterid]/title"/>
			</xsl:call-template>
			<xsl:call-template name="linefeed" />
		</xsl:if>
	</xsl:template>

	<xsl:template name="getFields">
		<xsl:param name="theCard"/>
		<xsl:param name="chaptername"/>

		<xsl:for-each select="$fieldsElement/fieldsToExport/fieldName">
			<xsl:text>"</xsl:text>
			<xsl:call-template name="getField">
				<xsl:with-param name="fieldname" select="./text()"/>
				<xsl:with-param name="theCard" select="$theCard"/>
				<xsl:with-param name="chaptername" select="$chaptername"/>
			</xsl:call-template>
			<xsl:text>"</xsl:text>
			<xsl:if test="position() != last()">
				<xsl:value-of select="$separator"/>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<xsl:template name="getField">
		<xsl:param name="fieldname"/>
		<xsl:param name="theCard"/>
		<xsl:param name="chaptername"/>

		<xsl:choose>
			<xsl:when test="$fieldname = 'answer'">
				<xsl:value-of select="normalize-space(translate($theCard/answer,'&#34;',''))"/>
			</xsl:when>
			<xsl:when test="$fieldname = 'question'">
				<xsl:value-of select="normalize-space(translate($theCard/question,'&#34;',''))"/>
			</xsl:when>
			<xsl:when test="$fieldname = 'answerexample'">
				<xsl:value-of select="normalize-space(translate($theCard/answerexample,'&#34;',''))"/>
			</xsl:when>
			<xsl:when test="$fieldname = 'questionexample'">
				<xsl:value-of select="normalize-space(translate($theCard/questionexample,'&#34;',''))"/>
			</xsl:when>
			<xsl:when test="$fieldname = 'answerdistractors'">
				<xsl:call-template name="exportDistractors">
					<xsl:with-param name="distractors" select="$theCard/answerdistractors" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$fieldname = 'questiondistractors'">
				<xsl:call-template name="exportDistractors">
					<xsl:with-param name="distractors" select="$theCard/questiondistractors" />
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="$fieldname = 'answerimage'">
				<xsl:choose>
					<xsl:when test="$copyFiles = 'true'">
						<xsl:value-of select="Export:GetLocalFile($theCard/answerimage/text(), 'Image')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$theCard/answerimage"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$fieldname = 'questionimage'">
				<xsl:choose>
					<xsl:when test="$copyFiles = 'true'">
						<xsl:value-of select="Export:GetLocalFile($theCard/questionimage/text(), 'Image')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$theCard/questionimage"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$fieldname = 'answeraudio'">
				<xsl:choose>
					<xsl:when test="$copyFiles = 'true'">
						<xsl:value-of select="Export:GetLocalFile($theCard/answeraudio/text(), 'Audio')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$theCard/answeraudio"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$fieldname = 'questionaudio'">
				<xsl:choose>
					<xsl:when test="$copyFiles = 'true'">
						<xsl:value-of select="Export:GetLocalFile($theCard/questionaudio/text(), 'Audio')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$theCard/questionaudio"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$fieldname = 'answerexampleaudio'">
				<xsl:choose>
					<xsl:when test="$copyFiles = 'true'">
						<xsl:value-of select="Export:GetLocalFile($theCard/answerexampleaudio/text(), 'Audio')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$theCard/answerexampleaudio"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$fieldname = 'questionexampleaudio'">
				<xsl:choose>
					<xsl:when test="$copyFiles = 'true'">
						<xsl:value-of select="Export:GetLocalFile($theCard/questionexampleaudio/text(), 'Audio')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$theCard/questionexampleaudio"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$fieldname = 'answervideo'">
				<xsl:choose>
					<xsl:when test="$copyFiles = 'true'">
						<xsl:value-of select="Export:GetLocalFile($theCard/answervideo/text(), 'Video')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$theCard/answervideo"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$fieldname = 'questionvideo'">
				<xsl:choose>
					<xsl:when test="$copyFiles = 'true'">
						<xsl:value-of select="Export:GetLocalFile($theCard/questionvideo/text(), 'Video')"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="$theCard/questionvideo"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
			<xsl:when test="$fieldname = 'chapter'">
				<xsl:value-of select="$chaptername"/>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template name="exportDistractors">
		<xsl:param name="distractors"/>
		<xsl:for-each select="$distractors/distractor">
			<xsl:value-of select="text()"/>
			<xsl:if test="position() != last()">
				<xsl:text>,</xsl:text>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>

	<xsl:template name="linefeed">
		<xsl:text>
</xsl:text>
	</xsl:template>
</xsl:stylesheet>