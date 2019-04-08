package ija;

import ija.schema.Block;
import ija.schema.Port;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyAdapter;
import java.awt.event.KeyEvent;
import java.beans.XMLDecoder;
import java.beans.XMLEncoder;
import java.io.*;
import java.util.ArrayList;

public class MainWindow {
    private JTabbedPane tabbedPane1;
    private JMenu menicko;
    private JPanel panel1;
    private JButton blokButton;
    private JButton relaceButton;
    private JButton schemaButton;
    private JButton smazatButton;
    private JButton spustitButton;
    private JButton ulozitButton;
    private JButton nacistButton;
    private JButton UpravBlockbutton;
    private JButton UpravSchemabutton;

    /*TODO- porty na schémata
*       RUN a Debug
*       vytvoření bloku popup-porty a datové typy
*       úprava bloku*/
    public MainWindow() {
        //TODO popup s oknem na parametry nového schématu
        schemaButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                //tabbedPane1.addTab("dalsi",new Canvas());
                //Canvas selected = (Canvas) tabbedPane1.getComponentAt(tabbedPane1.getSelectedIndex());

                SchemaPopup popup = new SchemaPopup(tabbedPane1);
                popup.run();

            }
        });

        //TODO popup s oknem na parametry nového bloku
        blokButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                Canvas selected = (Canvas) tabbedPane1.getComponentAt(tabbedPane1.getSelectedIndex());

                BlockPopup popup = new BlockPopup(selected);
                popup.run();

                /*
                HYNEK
                ArrayList<Port> testports= new ArrayList<Port>();
                testports.add(new Port("ayy",1.0));
                testports.add(new Port("ayy",3.0));
                testports.add(new Port("ayy",5.0));
                selected.addBlock(new Block("test","8", testports,new Port("ayy",0.1)));
                selected.repaint();
                */
            }
        });
        relaceButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                Canvas selected = (Canvas) tabbedPane1.getComponentAt(tabbedPane1.getSelectedIndex());
                selected.relate();
                selected.repaint();
            }
        });
        ulozitButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                try {
                    FileDialog fd = new FileDialog(new Frame() , "Uložit soubor", FileDialog.SAVE);
                    fd.setDirectory(".");
                    fd.setFile("*");
                    fd.setVisible(true);
                    String filename = fd.getFile();
                    if (filename == null)
                        return;
                    Canvas selected = (Canvas) tabbedPane1.getComponentAt(tabbedPane1.getSelectedIndex());
                    XMLEncoder encoder = new XMLEncoder(new BufferedOutputStream(new FileOutputStream(filename/*"."+ File.separator + tabbedPane1.getTitleAt(tabbedPane1.getSelectedIndex())+".xml"*/)));
                    encoder.writeObject(selected/*.getBlocks().get(0)*/);
                    encoder.close();

                } catch (FileNotFoundException e1) {
                    e1.printStackTrace();
                }


            }
        });
        nacistButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                try {
                    FileDialog fd = new FileDialog(new Frame() , "Načíst soubor", FileDialog.LOAD);
                    fd.setDirectory(".");
                    fd.setFile("*");
                    fd.setVisible(true);
                    String filename = fd.getFile();
                    if (filename == null)
                        return;
                    XMLDecoder decoder = new XMLDecoder(new BufferedInputStream(new FileInputStream(filename)));
                    Canvas loading = (Canvas) decoder.readObject();
                    tabbedPane1.addTab(filename, loading);
                } catch (FileNotFoundException e1) {
                    e1.printStackTrace();
                }
            }
        });
        UpravBlockbutton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                Canvas selected = (Canvas) tabbedPane1.getComponentAt(tabbedPane1.getSelectedIndex());
                BlockPopup popup = new BlockPopup(selected,selected.selectedBlock);
                popup.run();
            }
        });
        UpravSchemabutton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                SchemaPopup popup = new SchemaPopup(tabbedPane1,tabbedPane1.getSelectedComponent());
                popup.run();
            }
        });
        smazatButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                smazat();
            }
        });
    }
    private void smazat()
    {
        if (tabbedPane1.getTabCount() != 0) {
            Canvas selected = (Canvas) tabbedPane1.getComponentAt(tabbedPane1.getSelectedIndex());
            if(selected.selectedRel != null) {
                selected.getRelations().remove(selected.selectedRel);
                selected.selectedRel.delete();
                selected.repaint();
                return;
            }
            if(selected.selectedBlock != null)
            {
                selected.getBlocks().remove(selected.selectedBlock);
                selected.selectedBlock.delete();
                selected.repaint();
                return;
            }
            tabbedPane1.remove(tabbedPane1.getSelectedIndex());
        }
    }


    public static void main(String[] args) {
        JFrame frame = new JFrame("MainWindow");
        frame.setContentPane(new MainWindow().panel1);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.pack();
        frame.setVisible(true);
    }

}
