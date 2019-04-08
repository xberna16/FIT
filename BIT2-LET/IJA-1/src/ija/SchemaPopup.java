package ija;

import ija.schema.Block;
import ija.schema.Port;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyAdapter;
import java.awt.event.KeyEvent;
import java.util.ArrayList;

public class SchemaPopup {
    private JPanel panel1;
    private JButton CREATEButton;
    private JTextField NazovField;
    private JTextField numberField;
    private JTextField textField3;
    private JTextField textField2;
    private JTextField textField4;
    private JTextField textField5;
    private JTabbedPane tabbedPane;
    private int portNum = 0;
    private String issue;
    private JFrame frame;


    public SchemaPopup(JTabbedPane tabbedPane, Component tab){
        issue = "Uprava schematu";
        this.tabbedPane = tabbedPane;
        NazovField.setText(tabbedPane.getSelectedComponent().getName());

        //numberField.setText(POCET);
        //enable tolko kolko je inputPortov

        CREATEButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String nazov = NazovField.getText();
                tabbedPane.setTitleAt(tabbedPane.getSelectedIndex(), nazov);
                tabbedPane.getSelectedComponent().setName(nazov);
                frame.dispose();
            }
        });


    }



    public SchemaPopup(JTabbedPane tabbedPane){

        issue = "Vytvor schema";
        this.tabbedPane = tabbedPane;
        numberField.setText("0");
        enableZero();

        CREATEButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String nazov = NazovField.getText();
                tabbedPane.addTab(nazov,new Canvas());
                tabbedPane.getSelectedComponent().setName(nazov);
                frame.dispose();


            }
        });

        numberField.addKeyListener(new KeyAdapter() {
            @Override
            public void keyPressed(KeyEvent e) {
                super.keyPressed(e);
                if (e.getKeyCode()==KeyEvent.VK_ENTER){
                    try
                    {
                        portNum = Integer.parseInt(numberField.getText());
                    }
                    catch (NumberFormatException nfe)
                    {
                        System.out.println("NumberFormatException: " + nfe.getMessage());
                    }
                    if (portNum>3){
                        numberField.setText("3");
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

    public void run(){
        frame = new JFrame(issue);
        frame.setContentPane(this.panel1);
        frame.setDefaultCloseOperation(JFrame.DISPOSE_ON_CLOSE);
        frame.pack();
        frame.setVisible(true);
    }

    private void enableZero(){
        textField2.setEnabled(false);
        textField4.setEnabled(false);
        textField5.setEnabled(false);
    }
    private void enableOne(){
        textField2.setEnabled(true);
        textField4.setEnabled(false);
        textField5.setEnabled(false);
    }
    private void enableTwo(){
        textField2.setEnabled(true);
        textField4.setEnabled(true);
        textField5.setEnabled(false);
    }
    private void enableThree(){
        textField2.setEnabled(true);
        textField4.setEnabled(true);
        textField5.setEnabled(true);
    }

}
