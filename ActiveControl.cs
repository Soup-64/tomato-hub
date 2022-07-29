namespace avalonia_rider_test;

public interface ActiveControl
{ 
    /**
     * DESCRIPTION: for managing how widgets respond to no longer being in the foreground
     */
    public void changeActive(bool active);
}