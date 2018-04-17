<?php
/**
 * User: Hynek
 * Date: 23.02.2018
 * Time: 18:59
 */

if (count($argv)==2)
{
    if ($argv[1] == "--help")
    {
        print 'Skript typu filtr nacte ze standardniho vstupu zdrojovy kod v IPPcode18, zkontroluje lexikalni a syntaktickou spravnost kodu a vypise na standardni vystup XML reprezentaci programu\n';
        exit(0);
    }
    else
    {
        exit(10);
    }
}
else if (count($argv)!=1)
{
    if($argv[1] == "--loc")
    {
        if(substr($argv[2],0, strlen("--stats")) == "--stats" && count($argv)==3)
        {
            print 'TODO';
            exit(0);
        }
    }
    if(substr($argv[1],0, strlen("--stats")) == "--stats")
    {
        if(($argv[2] == "--comments" || $argv[2] == "--loc") && count($argv)==3)
        {
            print 'TODO';
            exit(0);
        }
        if((($argv[2] == "--comments" || $argv[2] == "--loc") && ($argv[3] == "--comments" || $argv[3] == "--loc")) && count($argv)==4)
        {
            print 'TODO';
            exit(0);
        }
    }
    if($argv[1] == "--comments")
    {
        if(substr($argv[2],0, strlen("--stats")) == "--stats" && count($argv)==3)
        {
            print 'TODO';
            exit(0);
        }
    }
    exit(10);
}
$simpleInst = array("CREATEFRAME", "PUSHFRAME", "POPFRAME", "RETURN", "BREAK");
$varNext = array("DEFVAR", "POPS", "MOVE", "INT2CHAR", "STRLEN", "TYPE", "NOT", "ADD", "SUB", "MUL", "IDIV", "LT", "GT", "EQ", "AND", "OR", "STRI2INT",  "CONCAT", "GETCHAR", "SETCHAR", "READ");
define("varSymb", 2);
define("varSymb2", 7);
define("varType", 20);
$labelNext = array("CALL", "LABEL", "JUMP", "JUMPIFEQ", "JUMPIFNEQ");
define("labelSymb2",3);
$symbNext = array("PUSHS", "WRITE", "DPRINT");
$debug = false;
$instCount=0;
function throwErr($id,$line="unknown")
{
    fprintf(STDERR,"%s",$line);
    exit($id);

}
function checkLabel($label,$param=1)
{

    preg_match("/[a-zA-Z_\-$&%*]+[a-zA-Z_\-$&%*0-9]*/",$label,$regexOut);
    if(is_array($regexOut))
        $final = $regexOut[0];
    else
        $final = $regexOut;
    if($final == $label)
        return array('arg'.$param => Array('@type' => 'label', '%' => $label));//TODO pole
    else
        throwErr(21,$label);
}
function checkType($type,$param=1)//TODO
{

    preg_match("/(string|int|bool)/",$type,$regexOut);
    if(is_array($regexOut))
        $final = $regexOut[0];
    else
        $final = $regexOut;
    if($final == $type)
    {
        return array('arg'.$param => Array('@type' => 'type', '%' => $type));//TODO něco s polem
    }
}
function checkSymb($symbol,$param=1)
{

    if(strpos($symbol,'@') !== false)
    {
        $firstPart = substr($symbol,0,strpos($symbol,'@'));
        switch($firstPart) {
            case "string":
                preg_match('/string@([^\p{Cn}\t\n \\\\]*(\\\\[0-9]{3})*)*/', $symbol, $regexOut);
                if (is_array($regexOut))
                    $final = $regexOut[0];
                else
                    $final = $regexOut;
                if ($final == $symbol) {
                    return array('arg' . $param => array('@type' => "string", '%' => substr($symbol, strpos($symbol, '@') + 1)));//TODO rozsekat a nacpat do pole
                }
                break;
            case "int":
                preg_match("/int@[-+]{0,1}[0-9]+/", $symbol, $regexOut);
                if (is_array($regexOut))
                    $final = $regexOut[0];
                else
                    $final = $regexOut;
                if ($final == $symbol) {
                    return array('arg' . $param => array('@type' => "int", '%' => substr($symbol, strpos($symbol, '@') + 1)));//TODO rozsekat a nacpat do pole
                } else
                {
                    fprintf(STDERR,"\n%s %s\n", $regexOut,$symbol);
                }
                break;

            case "bool":
                preg_match("/bool@(true|false)/",$symbol,$regexOut);
                if (is_array($regexOut))
                    $final = $regexOut[0];
                else
                    $final = $regexOut;
                if ($final == $symbol) {

                    return array('arg'.$param => array('@type'=>"bool",'%' => substr($symbol,strpos($symbol,'@')+1)));//TODO rozsekat a nacpat do pole
                }
                break;
            default :
                return checkVar($symbol,$param);
        }
    }
    throwErr(21,$symbol);
}
function checkVar($variable,$param=1)
{

    /*if(strpos($variable,'@') !== false)
    {
        $firstPart = substr($variable,0,strpos($variable,'@'));
        if( $firstPart == "GF" || $firstPart == "LF" || $firstPart == "TF")
        {*/
            //(GF|LF|TF)@[a-zA-Z_\-$&%*]+[a-zA-Z_\-$&%*0-9]*
            preg_match("/(GF|LF|TF)@[a-zA-Z_\-$&%*]+[a-zA-Z_\-$&%*0-9]*/",$variable,$regexOut);
    if(is_array($regexOut))
        $final = $regexOut[0];
    else
        $final = $regexOut;
            if($final == $variable)
            {
                return array('arg'.$param => array('@type'=>"var",'%' => $variable));//TODO něco s polem
            }
        /*}
    }*/
    throwErr(21, $variable);
}
function rempty ($var)
{
    return !($var == "" || $var == null);
}
function fetchLine($line)
{
    global $simpleInst, $varNext, $labelNext, $symbNext, $instCount;

	if(strlen($line)==0)
		return;
    $arr = array_filter(explode(' ',$line),'rempty');
    $arr = array_values($arr);
    if (empty($arr))
        return;
    $instruction = strtoupper($arr[0]);
    $instCount++;
    /*var_dump($line);
    var_dump($arr);*/
    if(count($arr) == 1)
    {
        if (in_array($instruction, $simpleInst))
        {
			return Array('instruction' => Array( '@order' => $instCount, '@opcode' => $instruction)); //TODO return Array
        }
        else
        throwErr(21,$line);
    }
    else
    {
        if (in_array($instruction, $varNext))
        {
            $firstParam = checkVar($arr[1]);//TODO posunout do XMLarray


            if(count($arr) == 2 && in_array($instruction, array_slice($varNext,0,varSymb)))
                return array('instruction' => Array( '@order' => $instCount, '@opcode' => $instruction, '%' => array(0=>$firstParam))); //TODO return Array
			if(in_array($instruction, array_slice($varNext,varSymb,varSymb2- varSymb)))
			{
				$secondParam = checkSymb($arr[2],2);
				if(count($arr) == 3)
                    return array('instruction' => Array( '@order' => $instCount, '@opcode' => $instruction, '%' => array(0=>$firstParam,1=>$secondParam))); //TODO return Array
			}
			if(in_array($instruction, array_slice($varNext,varSymb2,varType -varSymb2)))
			{
				$secondParam = checkSymb($arr[2],2);
				$thirdParam = checkSymb($arr[3],3);
				if(count($arr) == 4)
                    return array('instruction' => Array( '@order' => $instCount, '@opcode' => $instruction, '%' => array(0=>$firstParam,1=>$secondParam,2=>$thirdParam)));
			}
			if($instruction === "READ")
			{
				$secondParam = checkType($arr[2],2);//TODO posunout do XMLarray
				if(count($arr)==3)
                    return array('instruction' => Array( '@order' => $instCount, '@opcode' => $instruction, '%' => array(0=>$firstParam,1=>$secondParam)));
			}
			throwErr(21,$line);
        }
		if (in_array($instruction, $labelNext))
		{
			$firstParam = checkLabel($arr[1]);//TODO posunout do XMLarray
			if(count($arr) == 2 && in_array($instruction, array_slice($labelNext, 0, labelSymb2)))
                return array('instruction' => Array( '@order' => $instCount, '@opcode' => $instruction, '%' => array(0=>$firstParam)));
			if(in_array($instruction, array_slice($labelNext, labelSymb2, count($labelNext)-labelSymb2)))
			{
				$secondParam = checkSymb($arr[2],2);
				$thirdParam = checkSymb($arr[3],3);//TODO
				if(count($arr) == 4)
                    return array('instruction' => Array( '@order' => $instCount, '@opcode' => $instruction, '%' => array(0=>$firstParam,1=>$secondParam,2=>$thirdParam)));
			}
		}
		if (in_array($instruction, $symbNext))
		{
			$firstParam = checkSymb($arr[1]);//TODO
			if(count($arr) == 2)
                return array('instruction' => Array( '@order' => $instCount, '@opcode' => $instruction, '%' => array(0=>$firstParam)));
		}
    }
	throwErr(21,$line);
}
$input[] = array();
$output[] = array();
$i = 0;
while ($f = fgets(STDIN))
{
    $input[$i++] = trim(preg_replace('/\s+/',' ', $f));
    if (strpos($input[$i-1], '#') !== false)
    {
        $input[$i-1] = substr($input[$i-1], 0, strpos($input[$i-1], '#'));
    }
}
if (strtoupper( $input[0]) != strtoupper(".IPPcode18"))
    throwErr(21,"$input[0]");
for ($j = 1; $j < $i ; $j++)
{
    $output[$j]=fetchLine($input[$j]);
}
$output = array_filter($output,'rempty');

$xml = ArrToXML::ArrayToXML($output);
print $xml/*->buildXML(Array('@language'=> 'IPPcode18', '#' => $output))*/;
print "\n";

Class ArrToXML
{

    protected static function writeInst($val)
    {
           return '<instruction order="'.$val['@order'].'" opcode="'.$val['@opcode'].'">';
    }
    protected static function writeArg($val)
    {
        $subArr = $val[array_keys($val)[0]];

        return '<'.array_keys($val)[0].' type="'.$subArr['@type'].'">'.htmlspecialchars($subArr['%'],ENT_XML1).'</'.array_keys($val)[0].'>';
    }
    public static function ArrayToXml($arr)
    {
        $output = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
        $output .= '<program language="IPPcode18">';
        foreach($arr as $value)
        {
            $value = $value['instruction'];
            if (array_key_exists('%',$value))
            {
                $output .= self::writeInst($value);
                foreach($value['%'] as $item)
                {
                    $output .= self::writeArg($item);
                }
                $output .= '</instruction>';
            }
            else
            {
                $output .= '<instruction order="'.$value['@order'].'" opcode="'.$value['@opcode'].'"/>';
            }

        }
        $output .= '</program>';
        return $output;
    }

}