package ija.schema;
import javax.script.ScriptEngine;
import javax.script.ScriptEngineManager;
import javax.script.ScriptException;
import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class Block implements Serializable {
    public Boolean selected = false;
    public int x,y,width,height;
    private String name;
    private Boolean formErr = false;
    private String formula;
    private List<int[]> positions = new ArrayList<int[]>();
    private Port oPort;
    private ArrayList<Port> iPorts;
    private Boolean isValid = false;
    public Block()
    {}

    public void setSelected(Boolean selected) {
        this.selected = selected;
    }

    public void setX(int x) {
        this.x = x;
    }

    public void setY(int y) {
        this.y = y;
    }

    public void setWidth(int width) {
        this.width = width;
    }

    public void setHeight(int height) {
        this.height = height;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void setFormErr(Boolean formErr) {
        this.formErr = formErr;
    }

    public void setFormula(String formula) {
        this.formula = formula;
    }

    public void setPositions(List<int[]> positions) {
        this.positions = positions;
    }

    public void setOPort(Port oPort) {
        this.oPort = oPort;
    }

    public void setIPorts(ArrayList<Port> iPorts) {
        this.iPorts = iPorts;
    }

    public void setValid(Boolean valid) {
        isValid = valid;
    }

    public Boolean getSelected() {
        return selected;
    }

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    public int getWidth() {
        return width;
    }

    public int getHeight() {
        return height;
    }

    public Boolean getFormErr() {
        return formErr;
    }

    public String getFormula() {
        return formula;
    }

    public List<int[]> getPositions() {
        return positions;
    }

    public Boolean getValid() {
        return isValid;
    }

    public Port getOPort() {
        return oPort;
    }

    public ArrayList<Port> getIPorts() {
        return iPorts;
    }

    public enum lex {
        INIT, INT, DOUBLE, VAR, DIV, MUL, ADD, SUB, WHITE, END, ERROR, LBRACKET, RBRACKET;
    }

    public String getName() {
        return name;
    }
    private void lexer(String formula){
        lex state = lex.INIT;
        int var = -1;
        for (int index = 0; index < formula.length(); index++){
            char c = formula.charAt(index);
            // formula format is $1 SIGN $2 SIGN ... $N-1 SIGN $N;
            // SIGN={+, -, *, /}
            // it's also possible to use constants, for example: $1 * 3.14 + $2;
            switch (state) {
                case INIT:
                    if(c == '$'){
                        state = lex.VAR;
                        var = index;
                    }
                    else if(c >= '0' && c <= '9'){
                        state = lex.INT;
                    }
                    else if(c == '+'){
                        state = lex.ADD;
                    }
                    else if(c == '-'){
                        state = lex.SUB;
                    }
                    else if(c == '/'){
                        state = lex.DIV;
                    }
                    else if(c == '*'){
                        state = lex.MUL;
                    }
                    else if(c == ';'){
                        state = lex.END;
                    }
                    else if(c == ' '){
                        continue;
                    }
                    else if(c == '('){
                        state = lex.LBRACKET;
                    }
                    else if(c == ')'){
                        state = lex.RBRACKET;
                    }
                    else{
                        formErr = true;
                        return;
                    }
                    break;
                case ADD:
                    state = operator(c);
                    if(state == lex.ERROR) return;
                    break;
                case DIV:
                    state = operator(c);
                    if(state == lex.ERROR) return;
                    break;
                case INT:
                    if(c == ' '){
                        state = lex.INIT;
                    }
                    else if(c == '.'){
                        state = lex.DOUBLE;
                    }
                    else if(c >= '0' && c <= '9') {
                        continue;
                    }
                    else{
                        formErr = true;
                        return;
                    }
                    break;
                case MUL:
                    state = operator(c);
                    if(state == lex.ERROR) return;
                    break;
                case SUB:
                    state = operator(c);
                    if(state == lex.ERROR) return;
                    break;
                case VAR:
                    if(c >= '0' && c <= '9') {
                        continue;
                    }
                    else if(c == ' '){
                        state = lex.WHITE;
                    }
                    else if(c == ';'){
                        positions.add(new int[]{var, index});
                        var = -1;
                        state = lex.END;
                        break;
                    }
                    else{
                        formErr = true;
                        return;
                    }
                    break;
                case DOUBLE:
                    if(c >= '0' && c <= '9'){
                        continue;
                    }
                    else if(c == ' '){
                        state = lex.INIT;
                    }
                    else{
                        formErr = true;
                        return;
                    }
                    break;
                case WHITE:
                    positions.add(new int[]{var, index-1});
                    var = -1;
                    state = lex.INIT;
                    break;
                case LBRACKET:
                    if(c != ' '){
                        formErr = true;
                    }
                    state = lex.INIT;
                    break;
                case RBRACKET:
                    if(c != ' '){
                        formErr = true;
                    }
                    state = lex.INIT;
                    break;
                case END:
                    if(var != -1){
                        formErr = true;
                    }
                    return;
            }
        }
    }

    private lex operator(char c){
        if(c == ' '){
            return lex.INIT;
        }
        else{
            formErr = true;
            return lex.ERROR;
        }
    }

    public Block(String name, String formula, ArrayList<Port> iPorts, Port oPort){
        this.name = name;
        this.formula = formula;
        lexer(this.formula);
        this.iPorts = iPorts;
        this.oPort = oPort;
        x = 20;
        y= 20;
        width = 30 + (name.length()*8);
        height = 50;
    }

    public Boolean isValid(){
        checkValidity();
        return this.isValid;
    }

    private void checkValidity(){
        isValid = (positions.size() == iPorts.size() && !(formErr));
    }

    public double compute(){
        double val = 0;
        String form = this.formula;
        if(isValid()) {
            for (int i = positions.size(); i > 0; i--) {
                form = form.replace(form.substring(positions.get(i-1)[0], positions.get(i-1)[1]), Double.toString(iPorts.get(i-1).getVal()));
            }
            ScriptEngineManager mgr = new ScriptEngineManager();
            ScriptEngine engine = mgr.getEngineByName("JavaScript");
            try {
                val = (Double) engine.eval(form);
            }
            catch (ScriptException ex){
                System.out.print("something went wrong/n");
            }
        }
        return val;
    }

    public void changeFormula(String newFormula){
        positions.clear();
        formErr = false;
        this.formula = newFormula;
        lexer(newFormula);
    }
    public void delete(){
        for(Port p :iPorts)
        {
            if (p.occupied)
                p.getPlug().delete();
        }
        if (oPort.occupied)
            oPort.getPlug().delete();
    }

}
