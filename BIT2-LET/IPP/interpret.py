#coding=utf-8
import getopt
import sys
import re
import xml.etree.ElementTree as ET
from pathlib import Path


instructions = ["CREATEFRAME", "PUSHFRAME", "POPFRAME", "RETURN", "BREAK", "DEFVAR", "POPS", "MOVE", "INT2CHAR", "STRLEN", "TYPE", "NOT", "ADD", "SUB", "MUL", "IDIV", "LT", "GT", "EQ", "AND", "OR", "STRI2INT",  "CONCAT", "GETCHAR", "SETCHAR", "READ", "CALL", "LABEL", "JUMP", "JUMPIFEQ", "JUMPIFNEQ", "PUSHS", "WRITE", "DPRINT"]
labels = {}
GF = {"empty": "empty"}
LF = ['empty']
TF = {"NINIT": "NINIT"}
sstack = []


def getSymb(arg):
    if arg.get('type') == 'var':
        return getVar(arg)
    else:
        return arg.get('type') + '@' + xstr(arg.text)


def getVar(arg):
    global GF, LF, TF
    if arg.text[0:2] == "LF":
        if 'empty' in LF:
            exit(55)
        if arg.text[3:] in LF[0]:
            return LF[0].get(arg.text[3:])

    if arg.text[0:2] == "TF":
        if "NINIT" in TF:
            exit(55)
        if arg.text[3:] in TF:
            return TF.get(arg.text[3:])
    if arg.text[0:2] == "GF":
        if arg.text[3:] in GF:
            return GF.get(arg.text[3:])
    exit(54)


def setVar(arg, val):
    global GF, LF, TF
    if arg.text[0:2] == "LF":
        if 'empty' in LF:
            exit(55)
        if val == 'NINIT':
            if 'empty' in LF[0]:
                del LF[0]['empty']
            LF[0].update({arg.text[3:]: val})
            return
        elif 'empty' in LF:
            exit(55)
        elif arg.text[3:] in LF[0]:
            LF[0].update({arg.text[3:]: val})
            return

    if arg.text[0:2] == "TF":
        if "NINIT" in TF:
            exit(55)
        if 'empty' in TF:
            del TF['empty']
            TF.update({arg.text[3:]: val})
            return
        elif val == 'NINIT':
            TF.update({arg.text[3:]: val})
            return
        elif arg.text[3:] in TF:
            TF.update({arg.text[3:]: val})
            return
    if arg.text[0:2] == "GF":
        if 'empty' in GF:
            del GF['empty']
            GF.update({arg.text[3:]: val})
            return
        elif val == 'NINIT':
            if 'empty' in GF:
                del GF['empty']
            GF.update({arg.text[3:]: val})
            return
        elif arg.text[3:] in GF:
            GF.update({arg.text[3:]: val})
            return
    exit(54)


def xstr(s):
    if s is None:
        return ''
    return s


def checkParam(node, paramlist):
    i=0
    retval = {}
    for arg in node:
        if arg.tag != 'arg' + str(i+1):
            exit(31)
        if paramlist[i] == 'symb':
            if arg.get('type') not in ['bool', 'int', 'string']:
                paramlist[i] = 'var'
            else:
                if arg.get('type') == 'string':
                    if re.match('([^\t\n \\\\]*(\\\\[0-9]{3})*)*', xstr(arg.text), 0):
                        retval.update({arg.get('type'): xstr(arg.text)})
                    else:
                        exit(31)
                elif arg.get('type') == 'int':
                    if re.match("[-+]{0,1}[0-9]+", xstr(arg.text)):
                        retval.update({arg.get('type'): xstr(arg.text)})
                    else:
                        exit(31)
                else:
                    if re.match("(true|false)", xstr(arg.text)):
                        retval.update({arg.get('type'): xstr(arg.text)})
                    else:
                        exit(31)

        if paramlist[i] == 'var':
            if not re.match("(GF|LF|TF)@[a-zA-Z_\-$&%*]+[a-zA-Z_\-$&%*0-9]*", xstr(arg.text)):
                exit(52)

        elif paramlist[i] == 'label':

            if not re.match("[a-zA-Z_\-$&%*]+[a-zA-Z_\-$&%*0-9]*", xstr(arg.text)):
                exit(31)

        elif paramlist[i] == 'type':
            if not re.match("(bool|int|string)", xstr(arg.text)):
                exit(31)

        i += 1
    return True


def appendToFrame(frame, value):
    if 'NINIT' in frame:
        exit(55)
    if 'empty' in frame:
        del frame['empty']
    frame.update(value)
    return


def checkXML(xmlRoot):
    if xmlRoot.tag == 'program' and xmlRoot.attrib == {'language': 'IPPcode18'}:
        for child in xmlRoot:
            if child.tag == 'instruction':
                if child.get('opcode') in instructions:
                    if child.get('opcode') == "LABEL" and child.get('order') is not None:
                        if child.find('arg1') is None:
                            return False
                        if child.find('arg1').text in labels.keys():
                            exit(52)
                        labels[child.find('arg1').text] = child.get('order')
                for arg in child:
                    if arg.tag not in ["arg1", "arg2", "arg3"]:
                        return False
                continue
            return False
        return True
    return False


def parseArgs(arguments):
    if len(arguments) == 1:
        try:
            options, args = getopt.getopt(arguments, None, ['source=', 'help'])
        except getopt.GetoptError as err:
            print(err)
            exit(10)
        for opt, arg in options:
            if opt == '--source':
                if Path(arg).exists():
                    return arg
            if opt == '--help':
                print("Nápověda")
                exit(0)

    return False


def main():
    global TF, GF, LF, labels, sstack
    sourceFile = parseArgs(sys.argv[1:])
    if not sourceFile:
        exit(11)
    xmlRoot = ET.parse(sourceFile).getroot()
    if not checkXML(xmlRoot):
        exit(31)

    callList = []
    iDone=0
    i = 0
    while i < len(xmlRoot):
        iDone += 1
        child = xmlRoot[i]
        opcode = child.get('opcode')
        if int(xmlRoot[i].get('order')) != i+1:
            exit(31)
        if opcode == "CREATEFRAME":
            del TF
            TF = {"empty": "empty"}
        elif opcode == "PUSHFRAME":
            if "NINIT" in TF:
                exit(55)
            if 'empty' in LF:
                LF.remove('empty')
            LF.append(TF)
            del TF
            TF = {"NINIT": "NINIT"}
        elif opcode == "POPFRAME":
            if 'empty' in LF:
                exit(55)
            TF = LF.pop()
            if not LF:
                LF.append('empty')
        elif opcode == "RETURN":
            i = callList.pop(0)
            continue
        elif opcode == "BREAK":
            sys.stderr.write('Instruction: ' + str(i+1) + ' Instructions done: ' + str(iDone) + '\nGF-LF-TF:')
            sys.stderr.write(str(GF))
            sys.stderr.write(str(LF))
            sys.stderr.write(str(TF))
            sys.stderr.write('\n')
        elif opcode == "DEFVAR":
            checkParam(child, ["var"])
            setVar(child[0], 'NINIT')

        elif opcode == "POPS":
            if not sstack:
                exit(56)
            checkParam(child, ["var"])
            setVar(child[0], sstack.pop())

        elif opcode == "MOVE":
            checkParam(child, ["var", "symb"])
            a = getSymb(child[1])
            if 'NINIT' not in a:
                setVar(child[0], getSymb(child[1]))
            else:
                exit(54)

        elif opcode == "INT2CHAR":
            checkParam(child, ['var', 'symb'])
            a = getSymb(child[1])
            if 'NINIT' in a:
                exit(54)
            if a[0:3] == 'int':
                try:
                    setVar(child[0], chr(int(a[4:])))
                except ValueError:
                    exit(58)
            else:
                exit(53)
        elif opcode == "STRLEN":
            checkParam(child, ['var', 'symb'])
            a = getSymb(child[1])
            if 'NINIT' in a:
                exit(54)
            if a[0:6] == 'string':
                setVar(child[0], 'int@' + str(len(a[7:])))
            else:
                exit(53)
        elif opcode == "TYPE":
            checkParam(child, ['var', 'symb'])
            a = getSymb(child[1])
            if 'NINIT' in a:
                setVar(child[0], 'string@')
            else:
                setVar(child[0], 'string@' + a[0:a.find('@')])

        elif opcode == "NOT":
            checkParam(child, ['var', 'symb'])
            a = getSymb(child[1])
            if a[0:4] != 'bool':
                exit(53)
            if a[5:] == 'true':
                setVar(child[0], 'bool@false')
            else:
                setVar(child[0], 'bool@true')
        elif opcode == "ADD":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] != 'int' or b[0:3] != 'int':
                exit(53)
            setVar(child[0], 'int@' + str(int(a[4:]) + int(b[4:])))
        elif opcode == "SUB":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] != 'int' or b[0:3] != 'int':
                exit(53)
            setVar(child[0], 'int@' + str(int(a[4:]) - int(b[4:])))
        elif opcode == "MUL":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] != 'int' or b[0:3] != 'int':
                exit(53)
            setVar(child[0], 'int@' + str(int(a[4:]) * int(b[4:])))
        elif opcode == "IDIV":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] != 'int' or b[0:3] != 'int':
                exit(53)
            if int(b[4:]) == 0:
                exit(57)
            setVar(child[0], 'int@' + str(int(a[4:]) // int(b[4:])))
        elif opcode == "LT":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] == 'int' and b[0:3] == 'int':
                if int(a[4:]) < int(b[4:]):
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            elif a[0:4] == 'bool' and b[0:4] == 'bool':
                if a < b:
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            elif a[0:6] == 'string' and b[0:6] == 'string':
                if a < b:
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            else:
                exit(53)
        elif opcode == "GT":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] == 'int' and b[0:3] == 'int':
                if int(a[4:]) > int(b[4:]):
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            elif a[0:4] == 'bool' and b[0:4] == 'bool':
                if a > b:
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            elif a[0:6] == 'string' and b[0:6] == 'string':
                if a > b:
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            else:
                exit(53)
        elif opcode == "EQ":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] == 'int' and b[0:3] == 'int':
                if int(a[4:]) == int(b[4:]):
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            elif a[0:4] == 'bool' and b[0:4] == 'bool':
                if a == b:
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            elif a[0:6] == 'string' and b[0:6] == 'string':
                if a == b:
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            else:
                exit(53)
        elif opcode == "AND":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:4] == 'bool' and b[0:4] == 'bool':
                if a[5:] == 'true' and b[5:] == 'true':
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            else:
                exit(53)
        elif opcode == "OR":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:4] == 'bool' and b[0:4] == 'bool':
                if a[5:] == 'true' or b[5:] == 'true':
                    out = 'true'
                else:
                    out = 'false'
                setVar(child[0], 'bool@' + out)
            else:
                exit(53)
        elif opcode == "STRI2INT":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:6] != 'string' or b[0:3] != 'int':
                exit(53)
            if int(b[4:]) > len(a[7:]):
                exit(58)
            setVar(child[0], 'int@' + str(ord(a[int(b[4:])+7: int(b[4:])+8])))
        elif opcode == "CONCAT":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:6] == 'string' and b[0:6] == 'string':
                setVar(child[0], a+b[7:])
            else:
                exit(53)
        elif opcode == "GETCHAR":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:6] != 'string' or b[0:3] != 'int':
                exit(53)
            if int(b[4:]) > len(a[7:]):
                exit(58)
            setVar(child[0], 'string@' + a[int(b[4:])+7: int(b[4:]) + 8])

        elif opcode == "SETCHAR":
            checkParam(child, ['var', 'symb', 'symb'])
            a = getSymb(child[0])
            b = getSymb(child[1])
            c = getSymb(child[2])
            if b[0:3] != 'int' or c[0:6] != 'string':
                exit(53)
            if int(b[4:]) > len(a[7:]) or len(c[7:]) < 1:
                exit(58)
            setVar(child[0], 'string@' + a[7:int(b[4:])+7] + c[7:8] + a[int(b[4:])+8:])

        elif opcode == "READ":
            checkParam(child, ['var', 'type'])
            a = input()
            if child[1].text == 'int':
                try:
                    setVar(child[0], 'int@' + str(int(a)))
                except ValueError:
                    setVar(child[0], 'int@0')

            elif child[1].text == 'string':
                if re.match('([^\t\n \\\\]*(\\\\[0-9]{3})*)*', str(a)):
                    setVar(child[0], 'string@' + str(a))
                else:
                    setVar(child[0], 'string@')

            elif child[1].text == 'bool':
                if a.lower() == 'true':
                    setVar(child[0], 'bool@true')
                else:
                    setVar(child[0], 'bool@false')

        elif opcode == "CALL":
            checkParam(child, ['label'])
            callList.append(i+1)
            i = int(labels[child[0].text])
            continue
        elif opcode == "LABEL":
            a=1
            #print(opcode)
        elif opcode == "JUMP":
            checkParam(child, ['label'])
            i = int(labels[child[0].text])
            continue
        elif opcode == "JUMPIFEQ":
            checkParam(child, ['label', 'symb', 'symb'])
            doIt = False
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] == 'int' and b[0:3] == 'int':
                doIt = True
            if a[0:4] == 'bool' and b[0:4] == 'bool':
                doIt = True
            if a[0:6] == 'string' and b[0:6] == 'string':
                doIt = True
            if doIt:
                if a == b:
                    i = int(labels[child[0].text])
                    continue
            else:
                exit(53)

        elif opcode == "JUMPIFNEQ":
            checkParam(child, ['label', 'symb', 'symb'])
            doIt = False
            a = getSymb(child[1])
            b = getSymb(child[2])
            if a[0:3] == 'int' and b[0:3] == 'int':
                doIt = True
            if a[0:4] == 'bool' and b[0:4] == 'bool':
                doIt = True
            if a[0:6] == 'string' and b[0:6] == 'string':
                doIt = True
            if doIt:
                if a != b:
                    i = int(labels[child[0].text])
                    continue
            else:
                exit(53)

        elif opcode == "PUSHS":
            checkParam(child, ['symb'])
            sstack.append(getSymb(child[0]))
        elif opcode == "WRITE":
            checkParam(child, 'symb')
            a = getSymb(child[0])
            # while a.find('\\') != -1:
            # a = a[0:a.find('\\')+1] + chr(int(a[a.find('\\')+1:a.find('\\')+3])) + a[a.find('\\')+5:]
            print(a[a.find('@')+1:].encode('utf8').decode('ascii'))

        elif opcode == "DPRINT":
            checkParam(child, ['symb'])
            sys.stderr.write(getSymb(child[0]))
            sys.stderr.write('\n')
        else:
            print("Tohle se stát nemůže")
        i += 1


if __name__ == '__main__':
    main()
