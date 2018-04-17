<?php
/**
 * User: Hynek Bernard
 * Date: 12.04.2018
 * Time: 12:00:55
 */
unset($argv[0]);
$parametry = getopt('', array('help', 'directory:', 'recursive', 'parse-script:', 'int-script:'));
$parseScript = "parse.php";
$intScript = "interpret.py";
$directory = ".";
$recursive = false;
$tests;
function searchFold($path, $recursive)
{
	$outArr = Array();
	foreach(scandir($path) as $file)
	{
		if($file == "." || $file == "..")
			continue;
		if(strlen($file) > 4 && substr($file, -4) == ".src")
		{
			if (!file_exists($path."/".substr($file,0,-4).".in"))
				exec('touch '.$path."/".substr($file,0,-4).'.in');
			if (!file_exists($path."/".substr($file,0,-4).".out"))
				exec('touch '.$path."/".substr($file,0,-4).'.out');
			array_push ($outArr, $path."/".substr($file,0,-4));
		}
		if ($recursive && is_dir($path."/".$file))
		{
			$outArr = array_merge($outArr, searchFold($path."/".$file, $recursive));
		}
	}
	return $outArr;	
}
foreach ($parametry as $key => $val)
{
	switch($key){
	case "help": 
		print("parameters:\n--help\n--directory=<path>\n--recursive\n--parse-script=<file>\n--int-script=<file>"); 
		exit(0);
		break;
	case "directory":
		$directory = $val;
		break;
	case "recursive":
		$recursive = true;
		break;
	case "parse-script":
		$parseScript = $val;
		break;
	case "int-script":
		$intScript = $val;
		break;
	default:
		print("parameters:\n--help\n--directory=<path>\n--recursive\n--parse-script=<file>\n--int-script=<file>"); 
		exit(1);
		break;
	}
}
if (count($parametry) < count($argv))
{
	print("parameters:\n--help\n--directory=<path>\n--recursive\n--parse-script=<file>\n--int-script=<file>"); 
		exit(1);
}
if (!file_exists($parseScript))
{
	print("Parse script does not exist!!!\n");
	exit(2);
}
if (!file_exists($intScript))
{
	print("Interpreter script does not exist!!!\n");
	exit(2);
}
if(substr($directory,-1) == "/")
	$directory = substr($directory,0,-1);
if(file_exists($directory))
	$tests = searchFold($directory, $recursive);
else
{
	print("Directory does not exist!!!\n");
	exit(2);
}
$testResults;
echo "<!doctype html> <html> <table style='width:120px; height:30px'><tr><td id='SUCC' style='background-color:green'></td><td id='FAIL' style='background-color:red'></td></tr></table> <ul>";
$lasttest = "";
$firstFold = true;
$i = 0;
$succ = 0;
$fail = 0;
foreach($tests as $test)
{
	$i++;
	echo '<li style="border: 1px dotted black"><button type="button" onclick="if(document.getElementById(\'spoiler'.$i.'\') .style.display==\'none\') {document.getElementById(\'spoiler'.$i.'\') .style.display=\'\'}else{document.getElementById(\'spoiler'.$i.'\') .style.display=\'none\'}">'.$test."</button>";
	if (!file_exists($test.".rc"))
		exec('echo "0" >> '.$test.'.rc');
	exec('cat '.$test.'.src | php5.6 '.$parseScript.' >> '.$test.'.ref.in', $out, $rc);
	if($rc != 0)
	{
		exec('rm '.$test.'.ref.in');
		echo '<p style="color:red;">FAIL at parsing</p></li>';
		continue;
	}
	exec('python3.6 '.$intScript.' --source="'.$test.'.ref.in" >> '.$test.'.ref.out', $out, $rc);
	exec('echo "'.$rc.'" >> '.$test.'.ref.rc');
	exec('diff -q '.$test.'.ref.out '.$test.'.out',$out,$diff1);
	exec('diff -q '.$test.'.ref.in '.$test.'.in',$out,$diff2);
	exec('diff -q '.$test.'.ref.rc '.$test.'.rc',$out,$diff3);
	if($diff1 == 0 && $diff2 == 0 && $diff3 == 0)
	{
		echo '<p style="color:green;">OK</p>';
		$succ++;
	}
	else
	{
		echo '<p style="color:red;">FAIL</p>';
		$fail++;
	}
	
	echo '<div id="spoiler'.$i.'" style="border: 2px solid;display:none" ><xmp>'."SRC\n";
	system('cat '.$test.'.src 2>&1');
	echo "\nOUT\n";
	system('diff -y '.$test.'.ref.out '.$test.'.out 2>&1');
	echo "\nRC\n";
	system('diff -y '.$test.'.ref.rc '.$test.'.rc 2>&1');
	echo "</xmp></div>";
	exec('rm '.$test.'.ref.in');
	exec('rm '.$test.'.ref.out');
	exec('rm '.$test.'.ref.rc');
	echo "</li>";
}
echo "</ul><script>document.getElementById('SUCC').innerHTML = 'success:".$succ."';document.getElementById('FAIL').innerHTML = 'failed:".$fail."';</script></html>";
