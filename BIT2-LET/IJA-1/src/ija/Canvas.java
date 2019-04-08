package ija;

import ija.schema.Block;
import ija.schema.Port;
import ija.schema.Relation;

import java.awt.*;
import java.awt.event.*;
import java.io.Serializable;
import java.util.ArrayList;
import javax.swing.*;

public class Canvas extends JPanel implements MouseListener, Serializable {
    private ArrayList<Block> Blocks;
    private ArrayList<Relation> Relations;


    private static int portMargin = 10;
    private Point mousePt;
    private boolean relationCreate = false;
    private Port relOut = null;
    private Port relIn = null;
    public Block selectedBlock = null;
    public Relation selectedRel = null;


    public Block getSelectedBlock(){
        return selectedBlock;
    }

    public void stopCreation()
    {
        relationCreate = false;
        relIn = null;
        relOut = null;
    }


    public void setMousePt(Point mousePt) {
        this.mousePt = mousePt;
    }

    public void setBlocks(ArrayList<Block> blocks) {
        Blocks = blocks;
    }

    public void setRelations(ArrayList<Relation> relations) {
        Relations = relations;
    }

    public static void setPortMargin(int portMargin) {
        Canvas.portMargin = portMargin;
    }

    public void setRelationCreate(boolean relationCreate) {
        this.relationCreate = relationCreate;
    }

    public void setRelOut(Port relOut) {
        this.relOut = relOut;
    }

    public void setRelIn(Port relIn) {
        this.relIn = relIn;
    }

    public Point getMousePt() {
        return mousePt;
    }

    public ArrayList<Block> getBlocks() {
        return Blocks;
    }

    public ArrayList<Relation> getRelations() {
        return Relations;
    }

    public static int getPortMargin() {
        return portMargin;
    }

    public boolean isRelationCreate() {
        return relationCreate;
    }

    public Port getRelOut() {
        return relOut;
    }

    public Port getRelIn() {
        return relIn;
    }

    public void relate()
    {
        if (relationCreate) {
            stopCreation();
            return;
        }
        relationCreate = true;
    }
    @Override
    public void mouseClicked(MouseEvent e) {
        mousePt = e.getPoint();
        if (relationCreate) {
            for(Block b:Blocks)
            {
                b.selected = false;
                if(mousePt.x > b.x && mousePt.x < b.x+8 && mousePt.y > b.y+portMargin) {
                    int portIndex= (mousePt.y-b.y-portMargin)/10;
                    if (relOut != null)
                    {
                        if(relOut == b.getOPort() || b.getIPorts().get(portIndex).occupied)
                            return;
                        Relations.add(new Relation(b.getIPorts().get(portIndex),relOut));
                        relOut = null;
                        relationCreate = false;
                        repaint();
                        return;
                    }
                    relIn = b.getIPorts().get(portIndex);
                    if (relIn.occupied)
                        relIn = null;
                }
                else if(mousePt.x < b.x+b.width && mousePt.x > b.x+b.width-8 && mousePt.y > b.y+portMargin && mousePt.y < b.y+portMargin+8)
                {
                    if (relIn != null)
                    {
                        if(b.getIPorts().contains(relIn) || b.getOPort().occupied)
                            return;
                        Relations.add(new Relation(relIn,b.getOPort()));
                        relIn = null;
                        relationCreate = false;
                        repaint();
                        return;
                    }
                    relOut = b.getOPort();
                    if (relOut.occupied)
                        relOut = null;
                }
            }
            return;
        }
        Boolean select = true;
        selectedBlock = null;
        selectedRel = null;
        for(Block b:Blocks)
        {
            b.selected = false;
            if (select && mousePt.x > b.x-6 && mousePt.x < b.x+8 && mousePt.y > b.y && mousePt.y < b.y+b.height)
            {
                int portIndex= (mousePt.y-b.y-portMargin)/10;
                if (b.getIPorts().size() >= portIndex+1) {
                    select = false;
                    selectedRel = b.getIPorts().get(portIndex).getPlug();
                    selectedBlock = null;
                    continue;
                }
            }
            if (select && mousePt.x > b.x+b.width-8 && mousePt.x < b.x+b.width+6 && mousePt.y > b.y+portMargin && mousePt.y < b.y+portMargin+8)
            {
                int portIndex= (mousePt.y-b.y-portMargin)/10;
                    select = false;
                    selectedRel = b.getOPort().getPlug();
                    selectedBlock = null;
                    continue;
            }
            if(select && mousePt.x > b.x && mousePt.x < b.x+b.width && mousePt.y > b.y && mousePt.y < b.y+b.height) {
                b.selected = true;
                selectedBlock = b;
                selectedRel = null;
                select = false;
            }
        }
        repaint();
    }

    @Override
    public void mousePressed(MouseEvent e) {
        mousePt = e.getPoint();
        repaint();
    }

    @Override
    public void mouseReleased(MouseEvent e) {

    }

    @Override
    public void mouseEntered(MouseEvent e) {

    }

    @Override
    public void mouseExited(MouseEvent e) {

    }
    public void addBlock(Block block)
    {
        Blocks.add(block);
        int portY = block.y + portMargin;
        for(Port p:block.getIPorts())
        {
            p.x = block.x;
            p.y = portY;
            portY += portMargin;
        }
        Port o = block.getOPort();
        o.x = block.x+block.width-8;
        o.y = block.y+portMargin;
    }
    public void updateBlock(Block stary,Block novy)
    {
        Blocks.remove(stary);
        //stary = null;
        Blocks.add(novy);
        int portY = novy.y + portMargin;
        for(Port p:novy.getIPorts())
        {
            p.x = novy.x;
            p.y = portY;
            portY += portMargin;
        }
        Port o = novy.getOPort();
        o.x = novy.x+novy.width-8;
        o.y = novy.y+portMargin;
    }

    public Canvas() {
        Blocks = new ArrayList<Block>();
        Relations = new ArrayList<Relation>();
        this.addMouseListener(this);
        this.addMouseMotionListener(new MouseMotionAdapter() {

            @Override
            public void mouseMoved(MouseEvent e) {
                if (relationCreate) {
                    mousePt = e.getPoint();
                    repaint();
                }
            }

            @Override
            public void mouseDragged(MouseEvent e) {
                int dx = e.getX() - mousePt.x;
                int dy = e.getY() - mousePt.y;
                for(Block b:Blocks)
                {
                    if (b.selected)
                    {
                        b.x = b.x + dx;
                        b.y = b.y + dy;
                        int portY = b.y + portMargin;
                        for(Port p:b.getIPorts())
                        {
                            p.x = b.x;
                            p.y = portY;
                            portY += portMargin;
                        }
                        Port o = b.getOPort();
                        o.x = b.x+b.width-8;
                        o.y = b.y+portMargin;
                    }
                }
                mousePt = e.getPoint();
                repaint();
            }
        });
    }


    @Override
    public void paintComponent(Graphics g) {
        super.paintComponent(g);
        for(Block b:Blocks) {
            Port oPort = b.getOPort();
            g.setColor(Color.BLACK);
            g.drawRect(b.x - 1, b.y - 1, b.width + 1, b.height + 1);
            g.setColor(Color.LIGHT_GRAY);
            g.fillRect(b.x, b.y, b.width, b.height);
            g.setColor(Color.BLACK);
            g.drawString(b.getName(), b.x + (b.width / 4), b.y + (b.height / 2));
            if (b.selected) {
                g.setColor(Color.BLUE);
                g.drawRect(b.x - 2, b.y - 2, b.width + 3, b.height + 3);
            }
            for (Port p : b.getIPorts()) {
                if (p.occupied) {
                    g.setColor(Color.BLACK);
                    g.drawLine(p.x,p.y+4,p.x-6,p.y +4);
                    g.setColor(Color.GREEN);
                }
                else
                    g.setColor(Color.RED);
                g.fillRect(p.x,p.y, 8, 8);
                if (relationCreate && p == relIn)
                {
                    g.setColor(Color.BLACK);
                    g.drawLine(p.x,p.y+4,p.x-6,p.y +4);
                    g.drawLine(p.x-6,p.y +4, mousePt.x, mousePt.y);
                }
            }
            if (oPort.occupied) {
                if(oPort.getPlug() == selectedRel)
                    g.setColor(Color.MAGENTA);
                else
                    g.setColor(Color.BLACK);
                g.drawLine(b.x+b.width,b.y+12,b.x+b.width+6,b.y+12);
                g.drawLine(b.x+b.width+6,b.y+12,oPort.getPlug().getIPort().x-6,oPort.getPlug().getIPort().y+4);
                g.setColor(Color.GREEN);
            }

            else
                g.setColor(Color.RED);
            g.fillRect(b.x+b.width-8,b.y+portMargin,8,8);
            if (oPort == relOut)
            {
                g.setColor(Color.BLACK);
                g.drawLine(b.x+b.width,b.y+12,b.x+b.width+6,b.y+12);
                g.drawLine(b.x+b.width+6,b.y+12, mousePt.x, mousePt.y);
            }

            if (relationCreate)
            {
                g.setColor(Color.GREEN);
                g.fillRect(30,0,100,20);
                g.setColor(Color.BLACK);
                g.drawString("Tvorba relace",40,15);
            }
        }
    }

}
