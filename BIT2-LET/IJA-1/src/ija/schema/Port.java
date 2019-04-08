package ija.schema;

import ija.schema.Relation;

import java.io.Serializable;

public class Port implements Serializable {
    private String  dataType;
    private double value;
    public boolean occupied;
    private Relation plug;
    public int x,y;
    public Port()
    {

    }


    public void setDataType(String dataType) {
        this.dataType = dataType;
    }

    public void setValue(double value) {
        this.value = value;
    }

    public void setOccupied(boolean occupied) {
        this.occupied = occupied;
    }

    public void setPlug(Relation plug) {
        this.plug = plug;
    }

    public void setX(int x) {
        this.x = x;
    }

    public void setY(int y) {
        this.y = y;
    }

    public String getDataType() {
        return dataType;
    }

    public double getValue() {
        return value;
    }

    public boolean isOccupied() {
        return occupied;
    }

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    public Port(String dataType, double value){
        x=0;
        y=0;
        this.dataType = dataType;
        this.value = value;
        this.occupied = false;
    }

    public double getVal(){
        return this.value;
    }

    public void setVal( double value){
        this.value = value;
    }

    public String getType(){
        return this.dataType;
    }

    public boolean occupy( String dataType, Relation plug ){
        if ( this.occupied )
            return false;
        else{
            this.occupied = this.dataType.equals(dataType);
            if (this.occupied)
                this.plug = plug;
            return this.occupied;
        }
    }

    public Relation getPlug() {
        return plug;
    }

    public void free() {
        this.occupied = false;
        this.plug = null;
    }

    public void delete(){
        this.occupied = false;
        if (this.plug != null){
            plug.delete();
            plug = null;
        }
    }

}
