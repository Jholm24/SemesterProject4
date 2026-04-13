package dk.sdu.core.metadata;

import dk.sdu.core.enums.MachineType;
import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface ComponentMetadata {
    String name();
    String protocol();
    MachineType machineType();
    String description() default "";
    String icon() default "";
    int priority() default 0;
}
