<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"> 
<xsl:output method="html" />

<xsl:template match="/">
<xsl:value-of select="document(//book)"/>
</xsl:template>
</xsl:stylesheet>