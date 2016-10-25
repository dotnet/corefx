<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl exobj" xmlns:exobj="urn-myobject">

<xsl:output method="xml" indent="yes"/>

<xsl:template match="/">
    <out>
        <xsl:copy-of select="exobj:ReturnNodeSet('/books/book')/title" />
    </out>
</xsl:template>

</xsl:stylesheet>
