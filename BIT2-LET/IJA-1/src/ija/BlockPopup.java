package ija;

import ija.schema.Block;
import ija.schema.Port;

import javax.swing.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyAdapter;
import java.awt.event.KeyEvent;
import java.util.ArrayList;

public class BlockPopup {
        private String nazov;
        private JPanel panel1;
        private JButton Create;
        private JButton Close;
        private JTextField nazovField;
    private JTextField formulaField;
    private JTextField NumberField;
    private JTextField textField4;
    private JTextField textField5;
    private JTextField textField6;
    private JTextField textField1;
    private JTextField textField2;
    private JTextField textField3;
    private Canvas selectedCanvas;
    private Block selectedBlock;
    private int portNum = 3;
    private String issue;
    private JFrame frame;

        public BlockPopup(Canvas activeC, Block activeB){
            issue = "Uprava blocku";
            selectedCanvas = activeC;
            selectedBlock = activeB;

            nazovField.setText(activeB.getName());
            formulaField.setText(activeB.getFormula());
            ArrayList<Port> testports = new ArrayList<Port>();
            testports = activeB.getIPorts();
            portNum = testports.size();
            NumberField.setText(Integer.toString(portNum));

            if (portNum == 3){
                textField4.setText(testports.get(0).getDataType());
                textField5.setText(testports.get(1).getDataType());
                textField6.setText(testports.get(2).getDataType());

                enableThree();
            }
            else if (portNum == 2){
                textField4.setText(testports.get(0).getDataType());
                textField5.setText(testports.get(1).getDataType());

                enableTwo();
            }
            else if (portNum == 1){
                textField4.setText(testports.get(0).getDataType());

                enableOne();
            }
            else{
                portNum = 0;
                enableZero();
            }

            NumberField.addKeyListener(new KeyAdapter() {
                @Override
                public void keyPressed(KeyEvent e) {
                    super.keyPressed(e);
                    if (e.getKeyCode()==KeyEvent.VK_ENTER){
                        try
                        {
                            portNum = Integer.parseInt(NumberField.getText());
                        }
                        catch (NumberFormatException nfe)
                        {
                            System.out.println("NumberFormatException: " + nfe.getMessage());
                        }
                        if (portNum>3){
                            NumberField.setText("3");
                            portNum = 3;
                        }

                        if (portNum < 1){
                            enableZero();
                        }
                        else if (portNum == 1){  //ako to spravit tak ze to budem brat z JToolBaru?
                            enableOne();
                        }
                        else if(portNum == 2){
                            enableTwo();
                        }
                        else if(portNum == 3){
                            enableThree();
                        }

                    }
                }
            });

            Create.addActionListener(new ActionListener() {
                @Override
                public void actionPerformed(ActionEvent e) {
                    String nazov = nazovField.getText();
                    String formula = formulaField.getText();
                    ArrayList<Port> testports = new ArrayList<Port>();

                    if (portNum == 1){
                        testports.add(new Port(textField4.getText(),0.0 ) );
                    }
                    else if(portNum == 2){
                        testports.add(new Port(textField4.getText(),0.0 ) );
                        testports.add(new Port(textField5.getText(),0.0 ) );
                    }
                    else if (portNum == 3){
                        testports.add(new Port(textField4.getText(),0.0 ) );
                        testports.add(new Port(textField5.getText(),0.0 ) );
                        testports.add(new Port(textField6.getText(),0.0 ) );
                    }
                    selectedCanvas.updateBlock(selectedBlock ,new Block(nazov,formula, testports,new Port(textField1.getText(),0.0)));
                    selectedCanvas.repaint();
                    frame.dispose();
                }
            });
        }



        public BlockPopup(Canvas active) {

            issue = "Vytvor block";
            this.selectedCanvas = active;

            Create.addActionListener(new ActionListener() {
                @Override
                public void actionPerformed(ActionEvent e) {
                    String nazov = nazovField.getText();
                    String formula = formulaField.getText();
                    ArrayList<Port> testports = new ArrayList<Port>();

                    if (portNum == 1){  //ako to spravit tak ze to budem brat z JToolBaru?
                        testports.add(new Port(textField4.getText(),0.0 ));
                    }
                    else if(portNum == 2){
                        testports.add(new Port(textField4.getText(),0.0 ));
                        testports.add(new Port(textField5.getText(),0.0 ));
                    }
                    else if (portNum == 3){
                        testports.add(new Port(textField4.getText(),0.0 ));
                        testports.add(new Port(textField5.getText(),0.0 ));
                        testports.add(new Port(textField6.getText(),0.0 ));
                    }
                    selectedCanvas.addBlock(new Block(nazov,formula, testports,new Port(textField1.getText(),0.0)));
                    selectedCanvas.repaint();
                    frame.dispose();
                }
            });

            NumberField.addKeyListener(new KeyAdapter() {
                @Override
                public void keyPressed(KeyEvent e) {
                    super.keyPressed(e);
                    if (e.getKeyCode()==KeyEvent.VK_ENTER){
                        try
                        {
                            portNum = Integer.parseInt(NumberField.getText());
                        }
                        catch (NumberFormatException nfe)
                        {
                            System.out.println("NumberFormatException: " + nfe.getMessage());
                        }
                        if (portNum>3){
                            NumberField.setText("3");
                            portNum = 3;
                        }

                        if (portNum < 1){
                            enableZero();
                        }
                        else if (portNum == 1){  //ako to spravit tak ze to budem brat z JToolBaru?
                            enableOne();
                        }
                        else if(portNum == 2){
                            enableTwo();
                        }
                        else if(portNum == 3){
                            enableThree();
                        }

                    }
                }
            });
        }

        public void run() {
            frame = new JFrame(issue);
            frame.setContentPane(this.panel1);
            frame.setDefaultCloseOperation(JFrame.DISPOSE_ON_CLOSE);  // TRY ked v lavo hore dam X tak sa to neukonci, ak by som to tam nechal tak sa skonci cely program
            frame.pack();
            frame.setVisible(true);
        }


        private void enableZero(){
            textField4.setEnabled(false);
            textField5.setEnabled(false);
            textField6.setEnabled(false);
        }
        private void enableOne(){
            textField4.setEnabled(true);
            textField5.setEnabled(false);
            textField6.setEnabled(false);
        }
        private void enableTwo(){
            textField4.setEnabled(true);
            textField5.setEnabled(true);
            textField6.setEnabled(false);
        }
        private void enableThree(){
            textField4.setEnabled(true);
            textField5.setEnabled(true);
            textField6.setEnabled(true);
        }

    }


