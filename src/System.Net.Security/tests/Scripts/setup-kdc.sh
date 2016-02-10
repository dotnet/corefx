#!/bin/bash

OS=`cat /etc/os-release | grep "PRETTY_NAME" | sed 's/PRETTY_NAME=//g' | sed 's/["]//g' | awk '{print $1}'`

realm="TEST.COREFX.NET"

principal1="HOST/host.test.corefx.net"
principal2="HTTP"
krb_user="krb_user"
password="password"

kadmin="kadmin.local"
krb5kdc="krb5kdc"
kdb5_util="kdb5_util"
add_principal_cmd="add_principal -pw ${password}"

krb_conf="krb5.conf"
kdc_conf="kdc.conf"
keytabfile="/etc/krb5.keytab"

# TODO: These locations varies for different distros, Set the values conditianally
krb_conf_location="/etc/"
kdc_conf_location="/etc/krb5kdc/"
database_file="/var/lib/krb5kdc/principal*"

kdc_setup()
{
	#Create/copy krb5.conf in /etc/ and kdc.conf in /etc/krb5kdc/	
	echo "Copying krb5.conf and kdc.conf.."
	/bin/cp ${krb_conf} ${krb_conf_location}
	/bin/cp ${kdc_conf} ${kdc_conf_location}
	
	echo "Creating KDC database for realm ${realm}.."
	${kdb5_util} create -r ${realm} -P ${password} -s
	
	echo "Adding principal ${principal1}.."
	${kadmin} -q "${add_principal_cmd} ${principal1}@${realm}"
	
	echo "Adding principal ${principal2}.."
	${kadmin} -q "${add_principal_cmd} ${principal2}@${realm}"
	
	echo "Adding user ${krb_user}.."
	${kadmin} -q "${add_principal_cmd} ${krb_user}@${realm}"
	
	echo "Exporting keytab for ${principal1}"
	${kadmin} -q "ktadd ${principal1}@${realm}"
	
	echo "Exporting keytab for ${principal2}"
	${kadmin} -q "ktadd ${principal2}@${realm}"
	
	echo "Exporting keytab for ${krb_user}"
	${kadmin} -q "ktadd ${krb_user}@${realm}"
}

echo "Removing existing database"
rm -rf ${database_file}

case ${OS} in
	"Ubuntu")
		dpkg -s krb5-kdc >/dev/null 2>&1
		if [ $? -ne 0 ]
		then 
			echo "Installing krb5-kdc.."
			sudo DEBIAN_FRONTEND=noninteractive apt-get -y install krb5-kdc krb5-admin-server
		else
			echo "krb5-kdc already installed.."
		fi
		
		echo "Stopping KDC.."
		if pgrep krb5kdc 2> /dev/null; then killall krb5kdc ; fi
		if pgrep kadmind 2> /dev/null; then killall kadmind ; fi
		
		kdc_setup
		
		echo "Starting KDC.."
		${krb5kdc}
		
		;;
		
	"Debian")
		echo "This is a Debian system"
		;;

	"CentOS")
		echo "This is a CentOS system"
		;;

	"Red Hat")
		echo "This is a RedHat system"
		;;

	*)
		echo "This is an Unknown system"
		;;
esac
	
chmod +r ${keytabfile}
