# Bestelbons.Caliburn

Start the program in debug mode,
click the button "Bestelbons" on the right side.
And than hover over the "Approve" button at the bottom right ( don'r click on it). 
CheckCanApprove() should fire in  BestelbonsModel.cs but it doesn't

        public void CheckCanApprove()
        {
            string d = "test";
        }  Shoude fire

If you make a 'edit' between the  "" of the Message.Attach ( deleting a letter and retyping it) + hot reload

            <Button x:Name="Approve" Grid.Column="1"  Content="Approve"  Width="80" FontSize="12"  Background="#2D2D30" Style="{StaticResource BlueButton}"
                    cal:Message.Attach="[Event MouseEnter] = [Action CheckCanApprove]" />
                    
then the MouseEnter begins to fire
