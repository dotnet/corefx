<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:myObj="urn:my-objecT">


    <xsl:template match="/">

	<result>
		<xsl:value-of select="myObj:Fn1()"/>
	</result>
	
    </xsl:template>
</xsl:stylesheet>