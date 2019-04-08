package ija.schema;

import java.io.Serializable;

public class Relation implements Serializable{
    String dataType;
    double value;
    Port iPort, oPort;
    Boolean isValid;
    public Relation(){ }

    public String getDataType() {
        return dataType;
    }

    public void setDataType(String dataType) {
        this.dataType = dataType;
    }

    public double getValue() {
        return value;
    }

    public void setValue(double value) {
        this.value = value;
    }

    public void setIPort(Port iPort) {
        this.iPort = iPort;
    }

    public Port getOPort() {
        return oPort;
    }

    public void setOPort(Port oPort) {
        this.oPort = oPort;
    }

    public Boolean getValid() {
        return isValid;
    }

    public void setValid(Boolean valid) {
        isValid = valid;
    }

    public Port getIPort() {
        return iPort;
    }

    public Relation(Port inPort, Port outPort) {
        if (inPort == null || outPort == null)
        {
            isValid=false;
            return;
        }
        dataType = inPort.getType();
        isValid = inPort.occupy(dataType, this);
        if(!isValid)
        {
            return;
        }
        isValid = outPort.occupy(dataType, this);
        if(!isValid)
        {
            inPort.free();
            return;
        }
        iPort = inPort;
        oPort = outPort;
    }
    public void connect(){
        checkValidity();
        if (isValid) {
            value = iPort.getVal();
            oPort.setVal(value);
        }
    }
    public void delete(){
        iPort.free();
        oPort.free();
        isValid = false;
        iPort = null;
        oPort = null;
    }
    private void checkValidity(){
        isValid = iPort != null && oPort != null && iPort.occupied && oPort.occupied && iPort.getType().equals(dataType) && oPort.getType().equals(dataType);
    }
}
