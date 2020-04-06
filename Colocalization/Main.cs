using CellToolDK;
using System.Windows.Forms;
using Colocalization;

public class Main
{
    private Transmiter t;
    private TifFileInfo fi;

    private void ApplyChanges()
    {
        //Apply changes and reload the image
        t.ReloadImage();
    }

    public void Input(TifFileInfo fi, Transmiter t)
    {
        this.t = t;
        this.fi = fi;
        //Main entrance
        if (fi == null) return;
        if(!fi.available || !fi.loaded)
        {
            MessageBox.Show("Image is not avaliable!");
            return;
        }

        if (fi.sizeC <= 1)
        {
            MessageBox.Show("The image must have more then 1 color channels!");
            return;
        }
        
        if (fi.sizeC > 2)
        {
            MessageBox.Show("The image must have 2 color channels!");
            return;
        }

        if (fi.sizeZ > 1)
        {
            MessageBox.Show("The image must not be Z stack!");
            return;
        }

        MainForm form1 = new MainForm(fi);
        form1.Show();
    }
}
