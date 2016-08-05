<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
xmlns:date="http://exslt.org/dates-and-times" exclude-result-prefixes="date">
    <xsl:output indent="yes" omit-xml-declaration="yes"/>
    <xsl:template match="data">
        <out>
            <test1>
                <xsl:value-of select="date:second-in-minute(date)"/>                                
            </test1>
            <test2>
                <xsl:value-of select="date:second-in-minute(bad-date)"/>                                
            </test2>    
        </out>
    </xsl:template>
</xsl:stylesheet>

  