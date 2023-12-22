using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPhoto.Options;

[Verb("init", HelpText = "Generating a settings file and creating a bucket")]
public class InitOptions
{
    [Option("aws_access_key_id", HelpText = "AWS Access Key ID")]
    public string AwsAccessKeyId { get; set; }

    [Option("aws_secret_access_key", HelpText = "AWS Secret Access Key")]
    public string AwsSecretAccessKey { get; set; }

    [Option("bucket", HelpText = "S3 Bucket Name")]
    public string Bucket { get; set; }
}

/*Программа cloudphoto должна в интерактивном режиме запросить у пользователя значение параметров: 
 * aws_access_key_id, aws_secret_access_key, bucket и сохранить их в конфигурационном файле.

Если в облачном хранилище нет бакета с именем, которое указано в конфигурационном файле, 
то программа cloudphoto должна создать в облачном хранилище бакет с именем, которое указано в конфигурационном файле.*/